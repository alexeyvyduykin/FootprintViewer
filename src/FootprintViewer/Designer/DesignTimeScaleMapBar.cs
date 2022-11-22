using FootprintViewer.ViewModels;
using Mapsui;

namespace FootprintViewer.Designer;

public class DesignTimeScaleMapBar : ScaleMapBar
{
    public DesignTimeScaleMapBar() : base()
    {
        var viewport = new Viewport();
        viewport.SetSize(400, 200);
        viewport.SetResolution(2);

        ChangedPosition(new MPoint(50, 20));

        ChangedViewport(viewport);
    }
}
