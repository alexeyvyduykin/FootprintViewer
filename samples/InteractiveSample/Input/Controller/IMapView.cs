using InteractiveGeometry;
using Mapsui;
using Mapsui.UI;

namespace InteractiveSample.Input.Controller
{
    public interface IMapView : IView
    {
        IMapObserver MapObserver { get; set; }

        IController Controller { get; set; }

        MPoint ScreenToWorld(MPoint screenPosition);

        MPoint WorldToScreen(MPoint worldPosition);

        MapInfo GetMapInfo(MPoint screenPosition, int margin = 0);
    }
}