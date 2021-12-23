using Mapsui.Geometries;
using Mapsui.UI;

namespace InteractivitySample.Input.Controller
{
    public interface IMapView : IView
    {
        IMapObserver MapObserver { get; set; }

        IController Controller { get; set; }

        Point ScreenToWorld(Point screenPosition);

        Point WorldToScreen(Point worldPosition);

        MapInfo GetMapInfo(Point screenPosition, int margin = 0);
    }
}