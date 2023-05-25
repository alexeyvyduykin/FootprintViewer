using FootprintViewer.Fluent.Services2;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Utilities;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.Extensions;

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
        mapService.Navigator?.ZoomIn(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => mapService.ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private static void ZoomOut(this IMapService mapService, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.ZoomOut(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => mapService.ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private static void ZoomIn(this IMapService mapService, MPoint centerOfZoom, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.ZoomIn(centerOfZoom, duration, easing);
    }

    private static void ZoomOut(this IMapService mapService, MPoint centerOfZoom, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.ZoomOut(centerOfZoom, duration, easing);
    }

    private static void CenterOnCoordinate(this IMapService mapService, double lon, double lat, long duration = 0, Easing? easing = null)
    {
        var (x, y) = SphericalMercator.FromLonLat(lon, lat);

        mapService.Navigator?.CenterOn(x, y, duration, easing);
    }

    private static void CenterOn(this IMapService mapService, double x, double y, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.CenterOn(x, y, duration, easing);
    }

    private static void CenterOn(this IMapService mapService, MPoint center, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.CenterOn(center, duration, easing);
    }

    private static void NavigateTo(this IMapService mapService, MRect extent, ScaleMethod scaleMethod = ScaleMethod.Fit, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.NavigateTo(extent, scaleMethod, duration, easing);
    }

    private static void NavigateToFullEnvelope(this IMapService mapService, ScaleMethod scaleMethod = ScaleMethod.Fill, long duration = 0, Easing? easing = null)
    {
        mapService.Navigator?.NavigateToFullEnvelope(scaleMethod, duration, easing);
    }

    private static void FlyTo(this IMapService mapService, MPoint center, double maxResolution, long duration = 2000)
    {
        mapService.Navigator?.FlyTo(center, maxResolution, duration);

        Observable.StartAsync(() => mapService.ForceUpdate(duration + 100), RxApp.MainThreadScheduler).Subscribe();
    }

    private static async Task ForceUpdate(this IMapService mapService, long delay)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(delay));

        mapService.Map.ForceUpdate();
    }
}
