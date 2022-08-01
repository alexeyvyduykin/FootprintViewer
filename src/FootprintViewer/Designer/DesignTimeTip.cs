using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeTip : DrawingTip
    {
        public DesignTimeTip() : base(TipTarget.Rectangle)
        {
            HoverCreating(34545.432);

            InvalidateVisual();
        }
    }
}
