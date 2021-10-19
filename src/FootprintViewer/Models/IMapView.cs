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

        //IController ActualController { get; }

        MapInfo GetMapInfo(Point screenPosition, int margin = 0);

        void SetActiveTool(ToolType tool);

        void NavigateToAOI(BoundingBox boundingBox);

        void SetDescriptionAOI(string str);

        void SetDescriptionRoute(string str);

        void HideTracker();

        void InvalidatePlot(bool updateData = true);

        void SetClipboardText(string text);
    }
}