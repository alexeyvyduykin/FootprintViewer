using System;
using System.Linq;
using System.Reactive.Linq;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Splat;

namespace FootprintViewer
{
    public class ProjectFactory
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;

        public ProjectFactory(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
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
                        source.SelectFeature(name);
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

        public UserGeometryViewer CreateUserGeometryViewer()
        {
            var userGeometryViewer = new UserGeometryViewer(_dependencyResolver);

            return userGeometryViewer;
        }


    }
}
