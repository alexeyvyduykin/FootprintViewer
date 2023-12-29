using FootprintViewer.Layers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Models;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.UI.Services2;

public class MapService : IMapService
{
    private readonly Map _map;
    private readonly MapState _state;
    private readonly LayerStyleManager _styleManager = new();
    private readonly AreaOfInterest _aoi;
    private readonly Dictionary<LayerType, IProvider?> _providers = new();
    private readonly FeatureManager _featureManager = new();

    //private const string SelectField = Mapsui.Interactivity.InteractiveFields.Select;

    public MapService()
    {
        _state = new MapState();

        _map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        _map.Navigator.MouseWheelAnimation.Duration = 850;

        var editLayer = new EditLayer() { IsMapInfoLayer = false };

        _map.AddLayer(new Layer(), LayerType.WorldMap);
        _map.AddLayer(new WritableLayer(), LayerType.FootprintImage);
        _map.AddLayer(new Layer(), LayerType.GroundStation);
        _map.AddLayer(new Layer(), LayerType.GroundTarget);
        _map.AddLayer(new Layer(), LayerType.Sensor);
        _map.AddLayer(new Layer(), LayerType.Track);
        _map.AddLayer(new Layer(), LayerType.Footprint);
        _map.AddLayer(new WritableLayer(), LayerType.FootprintImageBorder);
        _map.AddLayer(editLayer, LayerType.Edit);
        _map.AddLayer(new VertexOnlyLayer(editLayer), LayerType.Vertex);
        _map.AddLayer(new Layer(), LayerType.User);

        foreach (var item in _map.Layers)
        {
            _styleManager.Select(item);
        }

        _aoi = new AreaOfInterest(_map);

        _featureManager = _featureManager
            .WithSelect(f => f["Select"] = true)
            .WithUnselect(f => f["Select"] = false)
            .WithEnter(f => f["Highlight"] = true)
            .WithLeave(f => f["Highlight"] = false);
    }

    public void AddLayerProvider(LayerType type, IProvider provider)
    {
        var layerName = type.ToString();

        switch (type)
        {
            case LayerType.GroundStation:
                {
                    var layer = new DynamicLayer(provider) { Name = layerName, IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.GroundTarget:
                {
                    if (provider is GroundTargetProvider gtProvider)
                    {
                        gtProvider.MaxVisible = _styleManager.MaxVisibleTargetStyle;
                    }

                    var layer = new DynamicLayer(provider, true) { Name = layerName, IsMapInfoLayer = true };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Sensor:
                {
                    var layer = new DynamicLayer(provider) { Name = layerName, IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Track:
                {
                    var layer = new DynamicLayer(provider) { Name = layerName, IsMapInfoLayer = false };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.Footprint:
                {
                    if (provider is FootprintProvider fProvider)
                    {
                        fProvider.MaxVisible = _styleManager.MaxVisibleFootprintStyle;
                    }

                    var layer = new DynamicLayer(provider, true)
                    {
                        Name = layerName,
                        // MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                        IsMapInfoLayer = true,
                    };
                    _styleManager.Select(layer);
                    _map.ReplaceLayer(layer, type);
                    break;
                }
            case LayerType.User:
                {
                    var layer = new DynamicLayer(provider) { Name = layerName, IsMapInfoLayer = true };
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
                return;
        }

        _providers.Add(type, provider);
    }

    public T? GetProvider<T>() where T : IProvider
    {
        try
        {
            foreach (var (key, item) in _providers)
            {
                if (item is T)
                {
                    return (T?)_providers[key];
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"Error is casting from IProvider to {typeof(T)}.");
        }

        return default;
    }

    public void EnterFeature(ILayer? layer, IFeature? feature)
    {
        _featureManager
            .OnLayer(layer)
            .Enter(feature);
    }

    public void LeaveFeature(ILayer? layer)
    {
        _featureManager
            .OnLayer(layer)
            .Leave();
    }

    public void SelectFeature(ILayer? layer, IFeature? feature)
    {
        _featureManager
            .OnLayer(layer)
            .Select(feature);
    }

    public void UnselectFeature(ILayer? layer)
    {
        _featureManager
            .OnLayer(layer)
            .Unselect();
    }

    public Map Map => _map;

    public MapState State => _state;

    public Navigator Navigator => _map.Navigator;

    public LayerStyleManager LayerStyle => _styleManager;

    public AreaOfInterest AOI => _aoi;
}
