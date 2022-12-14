using FootprintViewer.ViewModels.TimelinePanel;

namespace FootprintViewer.Designer;

public class DesignTimeTimelinePanelViewModel : TimelinePanelViewModel
{
    public DesignTimeTimelinePanelViewModel() : base(new DesignTimeData())
    {
        Show = true;
    }
}
