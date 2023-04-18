using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Factories;

public class MapFactory
{
    public Map CreateMap(LayerStyleManager styleManager, IDictionary<LayerType, IProvider> providers)
    {
        var map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        map.AddLayer(new Layer(), LayerType.WorldMap);
        map.AddLayer(new WritableLayer(), LayerType.FootprintImage);
        map.AddLayer(CreateGroundStationLayer(providers[LayerType.GroundStation]), LayerType.GroundStation);
        map.AddLayer(CreateTargetLayer(providers[LayerType.GroundTarget]), LayerType.GroundTarget);
        map.AddLayer(CreateSensorLayer(providers[LayerType.Sensor]), LayerType.Sensor);
        map.AddLayer(CreateTrackLayer(providers[LayerType.Track]), LayerType.Track);
        map.AddLayer(CreateFootprintLayer(providers[LayerType.Footprint]), LayerType.Footprint);
        map.AddLayer(CreateFootprintImageBorderLayer(), LayerType.FootprintImageBorder);
        map.AddLayer(CreateEditLayer(), LayerType.Edit);
        map.AddLayer(CreateVertexOnlyLayer(map), LayerType.Vertex);
        map.AddLayer(CreateUserLayer(providers[LayerType.User]), LayerType.User);

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

    private static ILayer CreateFootprintLayer(IProvider provider)
    {
        var layer = new DynamicLayer(provider, true)
        {
            //   MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateGroundStationLayer(IProvider provider)
    {
        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTrackLayer(IProvider provider)
    {
        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateTargetLayer(IProvider provider)
    {
        var layer = new DynamicLayer(provider, true)
        {
            //MaxVisible = styleManager.MaxVisibleTargetStyle,
            //DataSource = provider,
            IsMapInfoLayer = true,
        };

        return layer;
    }

    private static ILayer CreateSensorLayer(IProvider provider)
    {
        var layer = new DynamicLayer(provider)
        {
            IsMapInfoLayer = false,
        };

        return layer;
    }

    private static ILayer CreateUserLayer(IProvider provider)
    {
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
