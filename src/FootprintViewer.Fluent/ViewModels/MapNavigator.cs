using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Utilities;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels;

public class MapNavigator : IMapNavigator
{
    private readonly Map _map;

    public MapNavigator(Map map)
    {
        _map = map;
    }

    public void FlyToFootprintPreview(Geometry? geometry)
    {
        if (geometry != null)
        {
            FlyTo(geometry.Centroid.ToMPoint(), 100000);
        }
    }

    public void FlyToFootprint(Coordinate center)
    {
        var (x, y) = SphericalMercator.FromLonLat(center.X, center.Y);

        FlyTo(new MPoint(x, y), 100000);
    }

    public void ZoomIn()
    {
        ZoomIn(1000, Easing.SinOut);
    }

    public void ZoomOut()
    {
        ZoomOut(1000, Easing.SinOut);
    }

    private void ZoomIn(long duration = 0, Easing? easing = null)
    {
        Navigator?.ZoomIn(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private void ZoomOut(long duration = 0, Easing? easing = null)
    {
        Navigator?.ZoomOut(duration, easing);

        if (duration != 0)
        {
            Observable.StartAsync(() => ForceUpdate(duration + 50), RxApp.MainThreadScheduler).Subscribe();
        }
    }

    private void ZoomIn(MPoint centerOfZoom, long duration = 0, Easing? easing = null)
        => Navigator?.ZoomIn(centerOfZoom, duration, easing);

    private void ZoomOut(MPoint centerOfZoom, long duration = 0, Easing? easing = null)
        => Navigator?.ZoomOut(centerOfZoom, duration, easing);

    private void CenterOnCoordinate(double lon, double lat, long duration = 0, Easing? easing = null)
    {
        var (x, y) = SphericalMercator.FromLonLat(lon, lat);

        Navigator?.CenterOn(x, y, duration, easing);
    }

    private void CenterOn(double x, double y, long duration = 0, Easing? easing = null)
        => Navigator?.CenterOn(x, y, duration, easing);

    private void CenterOn(MPoint center, long duration = 0, Easing? easing = null)
        => Navigator?.CenterOn(center, duration, easing);

    private void NavigateTo(MRect extent, ScaleMethod scaleMethod = ScaleMethod.Fit, long duration = 0, Easing? easing = null)
        => Navigator?.NavigateTo(extent, scaleMethod, duration, easing);

    private void NavigateToFullEnvelope(ScaleMethod scaleMethod = ScaleMethod.Fill, long duration = 0, Easing? easing = null)
        => Navigator?.NavigateToFullEnvelope(scaleMethod, duration, easing);

    private void FlyTo(MPoint center, double maxResolution, long duration = 2000)
    {
        Navigator?.FlyTo(center, maxResolution, duration);

        Observable.StartAsync(() => ForceUpdate(duration + 100), RxApp.MainThreadScheduler).Subscribe();
    }

    private async Task ForceUpdate(long delay)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(delay));

        _map?.ForceUpdate();
    }

    public INavigator? Navigator { get; set; }

    public IReadOnlyViewport? Viewport { get; set; }
}
