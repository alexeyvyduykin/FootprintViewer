// --------------------------------------------------------------------------------------------------------------------
using Mapsui;
using Mapsui.Geometries;
using Mapsui.UI;

namespace FootprintViewer
{
    public interface IMapView : IView
    {
        IReadOnlyViewport Viewport { get; }

        IInteractiveFeatureObserver Observer { get; set; }

        IController Controller { get; set; }

        MapInfo GetMapInfo(Point screenPosition, int margin = 0);

        void NavigateToAOI(BoundingBox boundingBox);

        void HideTracker();

        void InvalidatePlot(bool updateData = true);

        void SetClipboardText(string text);
    }
}