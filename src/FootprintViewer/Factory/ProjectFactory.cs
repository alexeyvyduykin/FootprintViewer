using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
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

        private static ILayer CreateEditLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new EditLayer
            {
                Style = styleManager.EditStyle,
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

            return new BaseCustomLayer(source)
            {
                Style = styleManager.GroundStationStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITrackLayerSource>();

            return new BaseCustomLayer(source)
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

            return new BaseCustomLayer(source)
            {
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IUserLayerSource>();

            return new BaseCustomLayer(source)
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
    }
}
