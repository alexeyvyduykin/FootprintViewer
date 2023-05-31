using FootprintViewer.Factories;
using FootprintViewer.UI.Services2;
using FootprintViewer.Layers.Providers;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mapsui.Animations;

namespace FootprintViewer.UI.Extensions;

public static class MapServiceExtensions
{
    public static void FlyToFootprintPreview(this IMapService mapService, Geometry? geometry)
    {
        if (geometry != null)
        {
            mapService.FlyTo(geometry.Centroid.ToMPoint(), 100000);
        }
    }

    public static void FlyToFootprint(this IMapService mapService, Coordinate center)
    {
        var (x, y) = SphericalMercator.FromLonLat(center.X, center.Y);

        mapService.FlyTo(new MPoint(x, y), 100000);
    }

    public static void ZoomIn(this IMapService mapService)
    {
        mapService.ZoomIn(1000, Easing.SinOut);
    }

    public static void ZoomOut(this IMapService mapService)
    {
        mapService.ZoomOut(1000, Easing.SinOut);
    }

    private static void ZoomIn(this IMapService mapService, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomIn(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => mapService.ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private static void ZoomOut(this IMapService mapService, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomOut(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => mapService.ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private static void ZoomIn(this IMapService mapService, MPoint centerOfZoom, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomIn(centerOfZoom, duration, easing);
    }

    private static void ZoomOut(this IMapService mapService, MPoint centerOfZoom, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomOut(centerOfZoom, duration, easing);
    }

    private static void CenterOnCoordinate(this IMapService mapService, double lon, double lat, long duration = 0, Easing? easing = null)
    {
        var (x, y) = SphericalMercator.FromLonLat(lon, lat);

        mapService.Navigator.CenterOn(x, y, duration, easing);
    }

    private static void CenterOn(this IMapService mapService, double x, double y, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.CenterOn(x, y, duration, easing);
    }

    private static void CenterOn(this IMapService mapService, MPoint center, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.CenterOn(center, duration, easing);
    }

    private static void NavigateTo(this IMapService mapService, MRect extent, MBoxFit boxFit = MBoxFit.Fit, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomToBox(extent, boxFit, duration, easing);
    }

    private static void NavigateToFullEnvelope(this IMapService mapService, MBoxFit boxFit = MBoxFit.Fill, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator.ZoomToPanBounds(boxFit, duration, easing);
    }

    private static void FlyTo(this IMapService mapService, MPoint center, double maxResolution, long duration = 2000)
    {
        mapService.Navigator.FlyTo(center, maxResolution, duration);

        Observable.StartAsync(() => mapService.ForceUpdate(duration + 100), RxApp.MainThreadScheduler).Subscribe();
    }

    private static async Task ForceUpdate(this IMapService mapService, long delay)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(delay));

        mapService.Map.ForceUpdate();
    }

    public static void EnterFeature(this IMapService mapService, string name, LayerType type)
    {
        if (string.IsNullOrEmpty(name) == false)
        {
            switch (type)
            {
                case LayerType.GroundTarget:
                    {
                        var layerProvider = mapService.GetProvider<GroundTargetProvider>();
                        var layer = mapService.Map.GetLayer(LayerType.GroundTarget);
                        var feature = layerProvider?.Find(name, "Name");
                        mapService.EnterFeature(layer, feature);
                        break;
                    }
                case LayerType.Footprint:
                    {
                        var layerProvider = mapService.GetProvider<FootprintProvider>();
                        var layer = mapService.Map.GetLayer(LayerType.Footprint);
                        var feature = layerProvider?.Find(name, "Name");
                        mapService.EnterFeature(layer, feature);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"MapService.EnterFeature not have implement for {type} layer.");
                        break;
                    }
            }
        }
    }

    public static void LeaveFeature(this IMapService mapService, LayerType type)
    {
        switch (type)
        {
            case LayerType.GroundTarget:
                {
                    var layer = mapService.Map.GetLayer(LayerType.GroundTarget);
                    mapService.LeaveFeature(layer);
                    break;
                }
            case LayerType.Footprint:
                {
                    var layer = mapService.Map.GetLayer(LayerType.Footprint);
                    mapService.LeaveFeature(layer);
                    break;
                }
            default:
                {
                    Console.WriteLine($"MapService.LeaveFeature not have implement for {type} layer.");
                    break;
                }
        }
    }

    public static void SelectFeature(this IMapService mapService, string name, LayerType type)
    {
        if (string.IsNullOrEmpty(name) == false)
        {
            switch (type)
            {
                case LayerType.GroundTarget:
                    {
                        var layerProvider = mapService.GetProvider<GroundTargetProvider>();
                        var layer = mapService.Map.GetLayer(LayerType.GroundTarget);
                        var feature = layerProvider?.Find(name, "Name");
                        mapService.SelectFeature(layer, feature);
                        break;
                    }
                case LayerType.Footprint:
                    {
                        var layerProvider = mapService.GetProvider<FootprintProvider>();
                        var layer = mapService.Map.GetLayer(LayerType.Footprint);
                        var feature = layerProvider?.Find(name, "Name");
                        mapService.SelectFeature(layer, feature);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"MapService.SelectFeature not have implement for {type} layer.");
                        break;
                    }
            }
        }
    }
}
