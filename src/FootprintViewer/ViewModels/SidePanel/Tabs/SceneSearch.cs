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
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        private readonly ObservableAsPropertyHelper<List<FootprintPreview>> _footprints;
        private readonly FootprintPreviewProvider _footprintPreviewProvider;
        private readonly FootprintPreviewGeometryProvider _footprintPreviewGeometryProvider;
        private readonly Map _map;
        private readonly IDictionary<string, IGeometry> _geometries;
        private bool _firstLoading = true;

        public event EventHandler? CurrentFootprint;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            _map = dependencyResolver.GetExistingService<Map>();

            _footprintPreviewProvider = dependencyResolver.GetExistingService<FootprintPreviewProvider>();

            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();

            Title = "Поиск сцены";

            Filter = new SceneSearchFilter(dependencyResolver);

            _geometries = _footprintPreviewGeometryProvider.GetFootprintPreviewGeometries();

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null && _map != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    _map.Layers.Replace(nameof(LayerType.FootprintImage), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            MouseOverEnter = ReactiveCommand.Create<FootprintPreview?>(ShowFootprintBorder);

            MouseOverLeave = ReactiveCommand.Create(HideFootprintBorder);

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            SelectedItemChangedCommand = ReactiveCommand.Create<FootprintPreview>(SelectionChanged);

            Loading = ReactiveCommand.CreateFromTask<SceneSearchFilter?, List<FootprintPreview>>(LoadingAsync);

            Filter.Update.Select(filter => filter).InvokeCommand(Loading);

            // TODO: avoid from first loading design
            this.WhenAnyValue(s => s.IsActive).Where(active => active == true && _firstLoading == true).Select(_ => Filter).InvokeCommand(Loading);

            this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);

            this.WhenAnyValue(s => s.IsCompact).Where(c => c == true).Subscribe(_ => IsFilterOpen = false);

            _footprints = Loading.ToProperty(this, x => x.Footprints, scheduler: RxApp.MainThreadScheduler);
        }

        private ReactiveCommand<SceneSearchFilter?, List<FootprintPreview>> Loading { get; }

        private async Task<List<FootprintPreview>> LoadingAsync(SceneSearchFilter? filter = null)
        {
            _firstLoading = false;

            if (filter == null)
            {
                return await Task.Run(() =>
                {
                    return _footprintPreviewProvider.GetFootprintPreviews();
                });
            }
            else
            {
                return await Task.Run(() =>
                {
                    var list = _footprintPreviewProvider.GetFootprintPreviews();

                    return list.Where(s => filter.Filtering(s)).ToList();
                });
            }
        }

        public void SetAOI(IGeometry aoi) => Filter.AOI = aoi;

        public void ResetAOI() => Filter.AOI = null;

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

        public List<FootprintPreview> Footprints => _footprints.Value;

        [Reactive]
        public FootprintPreview? SelectedFootprint { get; set; }

        [Reactive]
        public SceneSearchFilter Filter { get; set; }

        [Reactive]
        public bool IsFilterOpen { get; private set; }
    }
}
