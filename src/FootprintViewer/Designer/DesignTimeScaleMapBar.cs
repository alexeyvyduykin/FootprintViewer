using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Extensions;

namespace FootprintViewer.Designer
{
    public class DesignTimeScaleMapBar : ScaleMapBar
    {
        public DesignTimeScaleMapBar() : base()
        {
            var viewport = new Viewport();
            viewport.SetSize(400, 200);
            viewport.SetResolution(2);

            Position = ChangedPosition(50, 20, viewport).ToMPoint();

            var (resolution, scaleText, scaleLength) = ChangedViewport(viewport);

            Resolution = resolution;
            Scale = scaleText;
            ScaleLength = scaleLength;
        }
    }
}
