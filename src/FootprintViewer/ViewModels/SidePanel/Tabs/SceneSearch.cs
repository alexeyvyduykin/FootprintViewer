using FootprintViewer.Data;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        private readonly IProvider<(string, NetTopologySuite.Geometries.Geometry)> _footprintPreviewGeometryProvider;
        private readonly Map _map;
        private readonly IMapNavigator _mapNavigator;
        private bool _firstLoading = true;
        private readonly ObservableAsPropertyHelper<IDictionary<string, NetTopologySuite.Geometries.Geometry>> _geometries;
        public event EventHandler? CurrentFootprint;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            // TODO: make _map as IMap
            //       _map = (Map)dependencyResolver.GetExistingService<IMap>();
            //       _mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();

            var footprintPreviewProvider = dependencyResolver.GetExistingService<IProvider<FootprintPreview>>();
            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<IProvider<(string, NetTopologySuite.Geometries.Geometry)>>();

            ViewerList = ViewerListBuilder.CreateViewerList(footprintPreviewProvider);

            Filter = new SceneSearchFilter(dependencyResolver);

            Title = "Поиск сцены";

            LoadFootprintPreviewGeometry = ReactiveCommand.CreateFromTask(_ => _footprintPreviewGeometryProvider.GetValuesAsync(null));

            FilterClick = ReactiveCommand.Create(FilterClickImpl);

            // TODO: duplicates           
            _geometries = LoadFootprintPreviewGeometry.Select(s => s.ToDictionary(s => s.Item1, s => s.Item2)).ToProperty(this, x => x.Geometries);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(LoadFootprintPreviewGeometry);

            this.WhenAnyValue(s => s.IsActive)
                .Where(active => active == true && _firstLoading == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(Filter.Init);

            Filter.Init.Select(_ => Filter).InvokeCommand(ViewerList.Loading);
            Filter.Init.Subscribe(_ => _firstLoading = false);

            // Filter

            this.WhenAnyValue(s => s.IsActive).Where(active => active == false).Subscribe(_ => IsFilterOpen = false);
            this.WhenAnyValue(s => s.IsExpanded).Where(c => c == false).Subscribe(_ => IsFilterOpen = false);

            //       ViewerList.MouseOverEnter.Subscribe(s => ShowFootprintBorder(s));
            //        ViewerList.MouseOverLeave.Subscribe(_ => HideFootprintBorder());

            //       Filter.Update.InvokeCommand(ViewerList.Loading);    
        }

        public void SetAOI(NetTopologySuite.Geometries.Geometry aoi) => ((SceneSearchFilter)Filter).AOI = aoi;

        public void ResetAOI() => ((SceneSearchFilter)Filter).AOI = null;

        public ReactiveCommand<Unit, Unit> FilterClick { get; }

        private ReactiveCommand<Unit, List<(string, NetTopologySuite.Geometries.Geometry)>> LoadFootprintPreviewGeometry { get; }

        private void ShowFootprintBorder(FootprintPreview footprint)
        {
            if (IsGeometry(footprint) == true)
            {
                var layer = _map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.Add(new GeometryFeature() { Geometry = ToGeometry(footprint) });
                    writableLayer.DataHasChanged();
                }
            }
        }

        private void HideFootprintBorder()
        {
            var layer = _map.GetLayer(LayerType.FootprintImageBorder);

            if (layer != null && layer is WritableLayer writableLayer)
            {
                writableLayer.Clear();
                writableLayer.DataHasChanged();
            }
        }

        private bool IsGeometry(FootprintPreview footprint)
        {
            return Geometries.ContainsKey(footprint.Name!);
        }

        private NetTopologySuite.Geometries.Geometry ToGeometry(FootprintPreview footprint)
        {
            return Geometries[footprint.Name!];
        }

        private void FilterClickImpl()
        {
            IsFilterOpen = !IsFilterOpen;
        }

        [Reactive]
        public IViewerList<FootprintPreview> ViewerList { get; private set; }

        [Reactive]
        public IFilter<FootprintPreview> Filter { get; private set; }

        [Reactive]
        public bool IsFilterOpen { get; private set; }

        public IDictionary<string, NetTopologySuite.Geometries.Geometry> Geometries => _geometries.Value;
    }
}
