using DynamicData;
using FootprintViewer.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        private readonly IList<FootprintPreview> _sourceFootprints = new List<FootprintPreview>();
        private readonly FootprintPreviewProvider _footprintPreviewProvider;
        private readonly FootprintPreviewGeometryProvider _footprintPreviewGeometryProvider;
        private readonly Map _map;
        private readonly IDictionary<string, IGeometry> _geometries;

        public event EventHandler? CurrentFootprint;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            _map = dependencyResolver.GetExistingService<Map>();       
            _footprintPreviewProvider = dependencyResolver.GetExistingService<FootprintPreviewProvider>();
            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();

            Title = "Поиск сцены";
            Name = "Scene";

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && _map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    _map.Layers.Replace(nameof(LayerType.FootprintImage), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            //this.WhenAnyValue(s => s.UserDataSource).Subscribe(_ => UserDataSourceChanged());

            MouseOverEnter = ReactiveCommand.Create<FootprintPreview?>(ShowFootprintBorder);

            MouseOverLeave = ReactiveCommand.Create(HideFootprintBorder);

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            SelectedItemChangedCommand = ReactiveCommand.Create<FootprintPreview>(SelectionChanged);

            Filter = new SceneSearchFilter(dependencyResolver);

            Filter.FromDate = DateTime.Today.AddDays(-1);
            Filter.ToDate = DateTime.Today.AddDays(1);

            Filter.Update += Filter_Update;

            _geometries = _footprintPreviewGeometryProvider.GetFootprintPreviewGeometries();

            UserDataSourceChanged();
        }

        private void Filter_Update(object? sender, EventArgs e)
        {
            if (sender is SceneSearchFilter filter)
            {
                IsUpdating = true;

                Footprints.Clear();

                foreach (var item in _sourceFootprints)
                {
                    if (filter.Filtering(item) == true)
                    {
                        Footprints.Add(item);
                    }
                }

                IsUpdating = false;
            }
        }

        public void SetAOI(IGeometry aoi)
        {
            Filter.AOI = aoi;

            Filter.ForceUpdate();
        }

        public void ResetAOI()
        {
            Filter.AOI = null;

            Filter.ForceUpdate();
        }

        private static async Task<IEnumerable<FootprintPreview>> LoadDataAsync(FootprintPreviewProvider provider)
        {
            return await Task.Run(() =>
            {              
                return provider.GetFootprintPreviews();
            });
        }

        private static IEnumerable<FootprintPreview> LoadData(FootprintPreviewProvider provider)
        {               
            return provider.GetFootprintPreviews();        
        }

        private async void UserDataSourceChanged()
        {
            var footprints = await LoadDataAsync(_footprintPreviewProvider);

            _sourceFootprints.AddRange(footprints);

            Footprints = new ObservableCollection<FootprintPreview>(footprints);

            var sortNames = new List<FootprintPreview>(footprints).Select(s => s.SatelliteName).Distinct().ToList();
            sortNames.Sort();

            Filter.AddSensors(sortNames);
        }

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        public ReactiveCommand<FootprintPreview?, Unit> MouseOverEnter { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

        public ReactiveCommand<FootprintPreview, Unit> SelectedItemChangedCommand { get; }

        private void ShowFootprintBorder(FootprintPreview? footprint)
        {
            if (footprint != null && IsGeometry(footprint) == true)
            {
                var layers = _map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new Feature() { Geometry = ToGeometry(footprint) });
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private bool IsGeometry(FootprintPreview footprint)
        {
            return _geometries.ContainsKey(footprint.Name);
        }

        private IGeometry ToGeometry(FootprintPreview footprint)
        {
            return _geometries[footprint.Name];
        }

        private Point GetCenter(FootprintPreview footprint)
        {
            return _geometries[footprint.Name].BoundingBox.Centroid;
        }

        private void HideFootprintBorder()
        {
            var layers = _map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

            if (layers != null)
            {
                var layer = layers.SingleOrDefault();

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.DataHasChanged();
                }
            }
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        private void SelectionChanged(FootprintPreview footprint)
        {
            if (footprint != null && IsGeometry(footprint) == true)
            {
                var point = GetCenter(footprint);

                _map.Initialized = false;

                _map.Home = (navigator) =>
                {
                    navigator.CenterOn(point);
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                _map.Layers.Add(layer);
                _map.Layers.Remove(layer);
            }
        }

        [Reactive]
        public ObservableCollection<FootprintPreview> Footprints { get; private set; } = new ObservableCollection<FootprintPreview>();

        [Reactive]
        public FootprintPreview? SelectedFootprint { get; set; }

        [Reactive]
        public SceneSearchFilter Filter { get; set; }

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        [Reactive]
        public bool IsUpdating { get; set; } = false;
    }
}
