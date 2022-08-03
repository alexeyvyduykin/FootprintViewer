using FootprintViewer.Configurations;
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
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();

            var mapBackgroundList = new MapBackgroundList(_dependencyResolver);

            mapBackgroundList.Loading.Where(s => s.Count != 0).Subscribe(s => { map.SetWorldMapLayer(s.First()); });

            mapBackgroundList.WorldMapChanged.Subscribe(s => map.SetWorldMapLayer(s));

            return mapBackgroundList;
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

            var targetLayer = map.GetLayer<TargetLayer>(LayerType.GroundTarget);

            var groundTargetViewer = new GroundTargetTab(_dependencyResolver);

            groundTargetViewer.SelectedItemObservable.Subscribe(groundTarget =>
            {
                if (groundTarget != null)
                {
                    var name = groundTarget.Name;

                    if (string.IsNullOrEmpty(name) == false)
                    {
                        targetLayer?.SelectFeature(name);
                    }
                }
            });

            groundTargetViewer.Enter.Subscribe(groundTarget =>
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    targetLayer?.ShowHighlight(name);
                }
            });

            groundTargetViewer.Leave.Subscribe(_ =>
            {
                targetLayer?.HideHighlight();
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
