using FootprintViewer.Layers;
using FootprintViewer.Styles;
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

        public Map CreateMap()
        {
            var map = new Map()
            {
                CRS = "EPSG:3857",
                //   Transformation = new MinimalTransformation(),
            };

            map.AddLayer(new Layer(), LayerType.WorldMap);
            map.AddLayer(new WritableLayer(), LayerType.FootprintImage);
            map.AddLayer(CreateGroundStationLayer(_dependencyResolver), LayerType.GroundStation);
            map.AddLayer(CreateTargetLayer(_dependencyResolver), LayerType.GroundTarget);
            map.AddLayer(CreateSensorLayer(_dependencyResolver), LayerType.Sensor);
            map.AddLayer(CreateTrackLayer(_dependencyResolver), LayerType.Track);
            map.AddLayer(CreateFootprintLayer(_dependencyResolver), LayerType.Footprint);
            map.AddLayer(CreateFootprintImageBorderLayer(_dependencyResolver), LayerType.FootprintImageBorder);
            map.AddLayer(CreateEditLayer(_dependencyResolver), LayerType.Edit);
            map.AddLayer(CreateVertexOnlyLayer(map, _dependencyResolver), LayerType.Vertex);
            map.AddLayer(CreateUserLayer(_dependencyResolver), LayerType.User);

            return map;
        }

        public IMapNavigator CreateMapNavigator()
        {
            return new MapNavigator();
        }

        private static ILayer CreateEditLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var source = dependencyResolver.GetExistingService<IEditLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new BaseCustomLayer<IEditLayerSource>(source)
            {
                Style = styleManager.EditStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateVertexOnlyLayer(Map map, IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var editLayer = map.GetLayer(LayerType.Edit);

            return new VertexOnlyLayer(editLayer!)
            {
                Style = styleManager.VertexOnlyStyle,
            };
        }

        private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var source = dependencyResolver.GetExistingService<IFootprintLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new FootprintLayer(source)
            {
                Style = styleManager.FootprintStyle,
                MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                IsMapInfoLayer = true,
            };
        }

        private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            return new BaseCustomLayer<IGroundStationLayerSource>(source)
            {
                Style = styleManager.GroundStationStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITrackLayerSource>();

            return new BaseCustomLayer<ITrackLayerSource>(source)
            {
                Style = styleManager.TrackStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITargetLayerSource>();

            return new TargetLayer(source)
            {
                Style = styleManager.TargetStyle,
                MaxVisible = styleManager.MaxVisibleTargetStyle,
                IsMapInfoLayer = true,
            };
        }

        private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ISensorLayerSource>();

            return new BaseCustomLayer<ISensorLayerSource>(source)
            {
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IUserLayerSource>();

            return new BaseCustomLayer<IUserLayerSource>(source)
            {
                IsMapInfoLayer = true,
                Style = styleManager.UserStyle,
            };
        }

        private static ILayer CreateFootprintImageBorderLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new WritableLayer
            {
                Style = styleManager.FootprintImageBorderStyle,
            };
        }

        public InfoPanel CreateInfoPanel()
        {
            return new InfoPanel();
        }

        public ScaleMapBar CreateScaleMapBar()
        {
            return new ScaleMapBar();
        }

        public WorldMapSelector CreateWorldMapSelector()
        {
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();

            var worldMapSelector = new WorldMapSelector(_dependencyResolver);

            worldMapSelector.Loading.Subscribe(s => { map.SetWorldMapLayer(s.First()); });

            worldMapSelector.WorldMapChanged.Subscribe(s => map.SetWorldMapLayer(s));

            return worldMapSelector;
        }

        public SceneSearch CreateSceneSearch()
        {
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var sceneSearch = new SceneSearch(_dependencyResolver);

            sceneSearch.ViewerList.SelectedItemObservable.Subscribe(footprint =>
            {
                if (footprint != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    map.ReplaceLayer(layer, LayerType.FootprintImage);

                    if (sceneSearch.Geometries.ContainsKey(footprint.Name!) == true)
                    {
                        mapNavigator.SetFocusToPoint(sceneSearch.Geometries[footprint.Name!].Centroid.ToMPoint());
                    }
                }
            });

            sceneSearch.ViewerList.MouseOverEnter.Subscribe(footprint =>
            {
                if (sceneSearch.Geometries.ContainsKey(footprint.Name!) == true)
                {
                    var layer = map.GetLayer(LayerType.FootprintImageBorder);

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new GeometryFeature() { Geometry = sceneSearch.Geometries[footprint.Name!] });
                        writableLayer.DataHasChanged();
                    }
                }
            });

            sceneSearch.ViewerList.MouseOverLeave.Subscribe(_ =>
            {
                var layer = map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.DataHasChanged();
                }
            });

            return sceneSearch;
        }

        public FootprintObserver CreateFootprintObserver()
        {
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var footprintObserver = new FootprintObserver(_dependencyResolver);

            footprintObserver.ViewerList.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

            return footprintObserver;
        }

        public GroundTargetViewer CreateGroundTargetViewer()
        {
            var source = _dependencyResolver.GetExistingService<ITargetLayerSource>();

            var groundTargetViewer = new GroundTargetViewer(_dependencyResolver);

            groundTargetViewer.ViewerList.SelectedItemObservable.Subscribe(groundTarget =>
            {
                if (groundTarget != null)
                {
                    var name = groundTarget.Name;

                    if (string.IsNullOrEmpty(name) == false)
                    {
                        source.SelectGroundTarget(name);
                    }
                }
            });

            groundTargetViewer.ViewerList.MouseOverEnter.Subscribe(groundTarget =>
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    source.ShowHighlight(name);
                }
            });

            groundTargetViewer.ViewerList.MouseOverLeave.Subscribe(_ =>
            {
                source.HideHighlight();
            });

            return groundTargetViewer;
        }

        public SatelliteViewer CreateSatelliteViewer()
        {
            var satelliteViewer = new SatelliteViewer(_dependencyResolver);

            return satelliteViewer;
        }

        public GroundStationViewer CreateGroundStationViewer()
        {
            var groundStationViewer = new GroundStationViewer(_dependencyResolver);

            return groundStationViewer;
        }

        public UserGeometryViewer CreateUserGeometryViewer()
        {
            var userGeometryViewer = new UserGeometryViewer(_dependencyResolver);

            return userGeometryViewer;
        }
    }
}
