using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer
{
    public class ProjectFactory
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;

        public ProjectFactory(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public LanguageManager CreateLanguageManager()
        {
            var languagesConfiguration = _dependencyResolver.GetExistingService<LanguagesConfiguration>();

            return new LanguageManager(languagesConfiguration);
        }

        public InfoPanel CreateInfoPanel()
        {
            return new InfoPanel();
        }

        public BottomPanel CreateBottomPanel()
        {
            return new BottomPanel(_dependencyResolver);
        }

        public IMapNavigator CreateMapNavigator()
        {
            return new MapNavigator();
        }

        public ScaleMapBar CreateScaleMapBar()
        {
            return new ScaleMapBar();
        }

        public MapBackgroundList CreateMapBackgroundList()
        {
            var loader = _dependencyResolver.GetExistingService<TaskLoader>();
            var provider = _dependencyResolver.GetExistingService<IProvider<MapResource>>();
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();

            var mapBackgroundList = new MapBackgroundList();

            mapBackgroundList.WorldMapChanged.Subscribe(s => map.SetWorldMapLayer(s));

            loader.AddTaskAsync(() => LoadingAsync());

            return mapBackgroundList;

            async Task LoadingAsync()
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                var maps = await provider.GetNativeValuesAsync(null);

                mapBackgroundList.Update(maps);

                var item = maps.FirstOrDefault();

                if (item != null)
                {
                    map.SetWorldMapLayer(item);
                }
            }
        }

        public MapLayerList CreateMapLayerList()
        {
            return new MapLayerList(_dependencyResolver);
        }

        public FootprintPreviewTab CreateFootprintPreviewTab()
        {
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var footprintPreviewTab = new FootprintPreviewTab(_dependencyResolver);

            footprintPreviewTab.SelectedItemObservable.Subscribe(footprint =>
            {
                if (footprint != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    map.ReplaceLayer(layer, LayerType.FootprintImage);

                    if (footprintPreviewTab.Geometries.ContainsKey(footprint.Name!) == true)
                    {
                        mapNavigator.SetFocusToPoint(footprintPreviewTab.Geometries[footprint.Name!].Centroid.ToMPoint());
                    }
                }
            });

            footprintPreviewTab.Enter.Subscribe(footprint =>
            {
                if (footprintPreviewTab.Geometries.ContainsKey(footprint.Name!) == true)
                {
                    var layer = map.GetLayer(LayerType.FootprintImageBorder);

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new GeometryFeature() { Geometry = footprintPreviewTab.Geometries[footprint.Name!] });
                        writableLayer.DataHasChanged();
                    }
                }
            });

            footprintPreviewTab.Leave.Subscribe(_ =>
            {
                var layer = map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.DataHasChanged();
                }
            });

            return footprintPreviewTab;
        }

        public FootprintTab CreateFootprintTab()
        {
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var tab = new FootprintTab(_dependencyResolver);

            tab.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

            return tab;
        }

        public GroundTargetTab CreateGroundTargetTab()
        {
            var map = _dependencyResolver.GetExistingService<IMap>();
            var layer = map.GetLayer<Layer>(LayerType.GroundTarget);
            var targetManager = layer?.BuildManager(() => ((TargetLayerSource)layer.DataSource!).GetFeatures());
            var groundTargetViewer = new GroundTargetTab(_dependencyResolver);

            groundTargetViewer.SelectedItemObservable.Subscribe(groundTarget =>
            {
                if (groundTarget != null)
                {
                    var name = groundTarget.Name;

                    if (string.IsNullOrEmpty(name) == false)
                    {
                        targetManager?.SelectFeature(name);
                    }
                }
            });

            groundTargetViewer.Enter.Subscribe(groundTarget =>
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    targetManager?.ShowHighlight(name);
                }
            });

            groundTargetViewer.Leave.Subscribe(_ =>
            {
                targetManager?.HideHighlight();
            });

            return groundTargetViewer;
        }

        public SatelliteTab CreateSatelliteTab()
        {
            return new SatelliteTab(_dependencyResolver);
        }

        public UserGeometryTab CreateUserGeometryTab()
        {
            return new UserGeometryTab(_dependencyResolver);
        }
    }
}
