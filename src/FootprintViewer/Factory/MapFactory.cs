using FootprintViewer.Layers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Splat;

namespace FootprintViewer;

public class MapFactory
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;

    public MapFactory(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    public Map CreateMap()
    {
        var styleManager = _dependencyResolver.GetExistingService<LayerStyleManager>();

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
        map.AddLayer(CreateFootprintImageBorderLayer(), LayerType.FootprintImageBorder);
        map.AddLayer(CreateEditLayer(), LayerType.Edit);
        map.AddLayer(CreateVertexOnlyLayer(map), LayerType.Vertex);
        map.AddLayer(CreateUserLayer(_dependencyResolver), LayerType.User);

        foreach (var item in map.Layers)
        {
            styleManager.Select(item);
        }

        return map;
    }

    public FeatureManager CreateFeatureManager()
    {
        return new FeatureManager()
            .WithSelect(f => f[InteractiveFields.Select] = true)
            .WithUnselect(f => f[InteractiveFields.Select] = false)
            .WithEnter(f => f["Highlight"] = true)
            .WithLeave(f => f["Highlight"] = false);
    }

    private static ILayer CreateEditLayer()
    {
        return new EditLayer()
        {
            IsMapInfoLayer = false,
        };
    }

    private static ILayer CreateVertexOnlyLayer(Map map)
    {
        var editLayer = map.GetLayer(LayerType.Edit);

        return new VertexOnlyLayer(editLayer!);
    }

    private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<FootprintProvider>();

        var layer = new DynamicLayer(provider, true)
        {
            //   MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<GroundStationProvider>();

        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<TrackProvider>();

        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<GroundTargetProvider>();

        var layer = new DynamicLayer(provider, true)
        {
            //MaxVisible = styleManager.MaxVisibleTargetStyle,
            //DataSource = provider,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<SensorProvider>();

        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<UserGeometryProvider>();

        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateFootprintImageBorderLayer()
    {
        return new WritableLayer();
    }
}
