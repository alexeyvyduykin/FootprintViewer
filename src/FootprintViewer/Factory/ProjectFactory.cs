using FootprintViewer.Data;
using FootprintViewer.Interactivity;
using FootprintViewer.Interactivity.Designers;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Splat;
using System.Linq;

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
            var mapProvider = _dependencyResolver.GetExistingService<MapProvider>();

            var map = new Map()
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation(),
            };

            map.Layers.Add(CreateWorldMapLayer());                                // WorldMap
            map.Layers.Add(CreateFootprintImageLayer());                          // FootprintImage
            map.Layers.Add(CreateTargetLayer(_dependencyResolver));               // GroundTarget
            map.Layers.Add(CreateSensorLayer(_dependencyResolver));               // Sensor
            map.Layers.Add(CreateTrackLayer(_dependencyResolver));                // Track
            map.Layers.Add(CreateFootprintLayer(_dependencyResolver));            // Footprint
            map.Layers.Add(CreateFootprintImageBorderLayer(_dependencyResolver)); // FootprintImageBorder      
            map.Layers.Add(CreateEditLayers(_dependencyResolver));                // Edit / VertexOnly
            map.Layers.Add(CreateUserLayer(_dependencyResolver));                 // User

            map.SetWorldMapLayer(mapProvider.GetMapResources().FirstOrDefault()!);

            return map;
        }

        private static ILayer CreateWorldMapLayer()
        {
            return new Layer()
            {
                Name = nameof(LayerType.WorldMap)
            };
        }

        private static ILayer CreateFootprintImageLayer()
        {
            return new WritableLayer()
            {
                Name = nameof(LayerType.FootprintImage)
            };
        }

        private static ILayer[] CreateEditLayers(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            var editLayer = new EditLayer
            {
                Name = nameof(LayerType.Edit),
                Style = styleManager.EditStyle,
                IsMapInfoLayer = true
            };

            var vertexOnlyLayer = new VertexOnlyLayer(editLayer)
            {
                Name = nameof(LayerType.Vertex),
                Style = styleManager.VertexOnlyStyle,
            };

            return new ILayer[] { editLayer, vertexOnlyLayer };
        }

        private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var source = dependencyResolver.GetExistingService<IFootprintLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new FootprintLayer(source)
            {
                Name = nameof(LayerType.Footprint),
                Style = styleManager.FootprintStyle,
                MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                IsMapInfoLayer = true,
            };
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITrackLayerSource>();

            return new BaseCustomLayer(source)
            {
                Name = nameof(LayerType.Track),
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
                Name = nameof(LayerType.GroundTarget),
                Style = styleManager.TargetStyle,
                IsMapInfoLayer = false,
                MaxVisible = styleManager.MaxVisibleTargetStyle,
            };
        }

        private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ISensorLayerSource>();

            return new BaseCustomLayer(source)
            {
                Name = nameof(LayerType.Sensor),
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var customProvider = dependencyResolver.GetExistingService<CustomProvider>();

            return new UserLayer(customProvider)
            {
                Name = nameof(LayerType.User),
                //DataSource = customProvider,
                IsMapInfoLayer = true,
                Style = styleManager.UserStyle,
            };
        }

        private static ILayer CreateFootprintImageBorderLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new WritableLayer
            {
                Name = nameof(LayerType.FootprintImageBorder),
                Style = styleManager.FootprintImageBorderStyle,
            };
        }

        public InfoPanel CreateInfoPanel()
        {
            return new InfoPanel();
        }

        public ILayer CreateInteractiveLayer(ILayer layer, IInteractiveObject obj)
        {
            var styleManager = _dependencyResolver.GetExistingService<LayerStyleManager>();

            if (obj is IDesigner designer)
            {
                return new InteractiveLayer(layer, designer)
                {
                    Name = "InteractiveLayer",
                    Style = styleManager.DesignerStyle,
                };
            }

            return new InteractiveLayer(layer, obj)
            {
                Name = "InteractiveLayer",
                Style = styleManager.DecoratorStyle,
            };
        }

        public ILayer CreateInteractiveSelectLayer(ILayer source, IFeature feature)
        {
            var styleManager = _dependencyResolver.GetExistingService<LayerStyleManager>();

            return new SelectLayer(source, feature)
            {
                Name = "InteractiveLayer",
                IsMapInfoLayer = true,
                Style = styleManager.SelectStyle,
            };
        }
    }
}
