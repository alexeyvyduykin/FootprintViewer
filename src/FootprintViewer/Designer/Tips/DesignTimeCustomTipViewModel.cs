using FootprintViewer.ViewModels.Tips;

namespace FootprintViewer.Designer;

public class DesignTimeCustomTipViewModel : CustomTipViewModel
{
    public DesignTimeCustomTipViewModel() : base()
    {
        Target = TipTarget.Rectangle;

        Mode = TipMode.HoverCreating;

        Value = 34545.432;
    }
}
