using FootprintViewer.Factories;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;

namespace FootprintViewer.Fluent.Services2;

public class MapService : IMapService
{
    private readonly Map _map;
    public INavigator? _navigator;
    public IReadOnlyViewport? _viewport;
    private readonly LayerStyleManager _styleManager = new();

    public MapService()
    {
        _map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        _map.AddLayer(new Layer(), LayerType.WorldMap);
        _map.AddLayer(new Layer(), LayerType.FootprintImage);
        _map.AddLayer(new Layer(), LayerType.GroundStation);
        _map.AddLayer(new Layer(), LayerType.GroundTarget);
        _map.AddLayer(new Layer(), LayerType.Sensor);
        _map.AddLayer(new Layer(), LayerType.Track);
        _map.AddLayer(new Layer(), LayerType.Footprint);
        _map.AddLayer(new Layer(), LayerType.FootprintImageBorder);
        _map.AddLayer(new Layer(), LayerType.Edit);
        _map.AddLayer(new Layer(), LayerType.Vertex);
        _map.AddLayer(new Layer(), LayerType.User);

        foreach (var item in _map.Layers)
        {
            _styleManager.Select(item);
        }
    }

    public void AddLayerProvider(LayerType type, IProvider provider)
    {
        switch (type)
        {
            case LayerType.GroundStation:
                {
                    var layer = new DynamicLayer(provider) { IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.GroundTarget:
                {
                    var layer = new DynamicLayer(provider, true) { IsMapInfoLayer = true };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Sensor:
                {
                    var layer = new DynamicLayer(provider) { IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Track:
                {
                    var layer = new DynamicLayer(provider) { IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Footprint:
                {
                    var layer = new DynamicLayer(provider, true)
                    {
                        // MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                        IsMapInfoLayer = true,
                    };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.User:
                {
                    var layer = new DynamicLayer(provider) { IsMapInfoLayer = true };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.WorldMap:
            case LayerType.FootprintImage:
            case LayerType.FootprintImageBorder:
            case LayerType.Edit:
            case LayerType.Vertex:
            case LayerType.InteractiveLayer:
            default:
                Console.WriteLine($"Provider for {type} type layer not support.");
                break;
        }
    }

    public void SetNavigator(INavigator navigator)
    {
        _navigator = navigator;
    }

    public void SetViewport(IReadOnlyViewport viewport)
    {
        _viewport = viewport;
    }

    public IMap Map => _map;

    public INavigator? Navigator => _navigator;

    public IReadOnlyViewport? Viewport => _viewport;
}
