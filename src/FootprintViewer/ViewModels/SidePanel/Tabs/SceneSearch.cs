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
        // protected readonly SourceList<Footprint> _sourceFootprints;
        // private readonly ReadOnlyObservableCollection<Footprint> _footprints;

        public event EventHandler? CurrentFootprint;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetService<Map>();
            var userDataSource = dependencyResolver.GetService<IUserDataSource>();

            Title = "Поиск сцены";
            Name = "Scene";

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && Map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintImage), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            this.WhenAnyValue(s => s.UserDataSource).Subscribe(_ => UserDataSourceChanged());

            MouseOverEnter = ReactiveCommand.Create<FootprintPreview?>(ShowFootprintBorder);

            MouseOverLeave = ReactiveCommand.Create(HideFootprintBorder);

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            SelectedItemChangedCommand = ReactiveCommand.Create<FootprintPreview>(SelectionChanged);

            Filter = new SceneSearchFilter(dependencyResolver);

            Filter.FromDate = DateTime.Today.AddDays(-1);
            Filter.ToDate = DateTime.Today.AddDays(1);

            Filter.Update += Filter_Update;

            //_sourceFootprints = new SourceList<Footprint>();

            //var cancellation = _sourceFootprints.Connect()
            //    .Filter(Filter.Observable)
            //    .Bind(out _footprints)
            //    .DisposeMany()
            //    .Subscribe(_ =>
            //    {
            //        Task.Run(() => longRunningRoutine());
            //    });        

            UserDataSource = userDataSource;
            Map = map;
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

        private void longRunningRoutine()
        {
            IsUpdating = true;

            //  System.Threading.Thread.Sleep(1000);

            IsUpdating = false;
        }

        private static async Task<IEnumerable<FootprintPreview>> LoadDataAsync(IUserDataSource dataSource)
        {
            return await Task.Run(() =>
            {
                //Thread.Sleep(5000);
                return dataSource.GetFootprints();
            });
        }

        private async void UserDataSourceChanged()
        {
            if (UserDataSource != null)
            {
                var footprints = await LoadDataAsync(UserDataSource);

                // _sourceFootprints.Clear();
                // _sourceFootprints.AddRange(footprints);

                _sourceFootprints.AddRange(footprints);

               // Footprints.Clear();
               // Footprints.AddRange(footprints);

                Footprints = new ObservableCollection<FootprintPreview>(footprints);

                var sortNames = new List<FootprintPreview>(footprints).Select(s => s.SatelliteName).Distinct().ToList();
                sortNames.Sort();

                Filter.AddSensors(sortNames);
            }
        }

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        public ReactiveCommand<FootprintPreview?, Unit> MouseOverEnter { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeave { get; }

        public ReactiveCommand<FootprintPreview, Unit> SelectedItemChangedCommand { get; }

        private void ShowFootprintBorder(FootprintPreview? footprint)
        {
            if (Map != null && footprint != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

                if (layers != null)
                {
                    var layer = layers.SingleOrDefault();

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new Feature() { Geometry = footprint.Geometry });
                        writableLayer.DataHasChanged();
                    }
                }
            }
        }

        private void HideFootprintBorder()
        {
            if (Map != null)
            {
                var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintImageBorder));

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
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        private void SelectionChanged(FootprintPreview footprint)
        {
            if (Map != null && footprint != null && footprint.Geometry != null)
            {
                var point = footprint.Geometry.BoundingBox.Centroid;

                Map.Initialized = false;

                Map.Home = (navigator) =>
                {
                    navigator.CenterOn(point);
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                Map.Layers.Add(layer);
                Map.Layers.Remove(layer);
            }
        }

        [Reactive]
        public IUserDataSource? UserDataSource { get; set; }

        [Reactive]
        public Map? Map { get; set; }

        // public ReadOnlyObservableCollection<Footprint> Footprints => _footprints;

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
