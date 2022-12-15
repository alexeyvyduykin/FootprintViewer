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

    public FeatureManager CreateFeatureManager()
    {
        return new FeatureManager()
            .WithSelect(f => f[InteractiveFields.Select] = true)
            .WithUnselect(f => f[InteractiveFields.Select] = false)
            .WithEnter(f => f["Highlight"] = true)
            .WithLeave(f => f["Highlight"] = false);
    }

    private static ILayer CreateEditLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        return new EditLayer()
        {
            Style = styleManager.GetStyle(LayerType.Edit),
            IsMapInfoLayer = false,
        };
    }

    private static ILayer CreateVertexOnlyLayer(Map map, IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var editLayer = map.GetLayer(LayerType.Edit);

        return new VertexOnlyLayer(editLayer!)
        {
            Style = styleManager.GetStyle(LayerType.Vertex),
        };
    }

    private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<FootprintProvider>();

        provider.MaxVisible = styleManager.MaxVisibleFootprintStyle;

        var layer = new DynamicLayer(provider, true)
        {
            Style = styleManager.GetStyle(LayerType.Footprint),
            //   MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<GroundStationProvider>();

        var layer = new DynamicLayer(provider)
        {
            Style = styleManager.GetStyle(LayerType.GroundStation),
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<TrackProvider>();

        var layer = new DynamicLayer(provider)
        {
            Style = styleManager.GetStyle(LayerType.Track),
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<GroundTargetProvider>();

        provider.MaxVisible = styleManager.MaxVisibleTargetStyle;

        var layer = new DynamicLayer(provider, true)
        {
            Style = styleManager.GetStyle(LayerType.GroundTarget),
            //MaxVisible = styleManager.MaxVisibleTargetStyle,
            //DataSource = provider,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<SensorProvider>();

        var layer = new DynamicLayer(provider)
        {
            Style = styleManager.GetStyle(LayerType.Sensor),
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
        var provider = dependencyResolver.GetExistingService<UserGeometryProvider>();

        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = true,
            Style = styleManager.GetStyle(LayerType.User),
        };

        return layer;
    }

    private static ILayer CreateFootprintImageBorderLayer(IReadonlyDependencyResolver dependencyResolver)
    {
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        return new WritableLayer
        {
            Style = styleManager.GetStyle(LayerType.FootprintImageBorder),
        };
    }
}
