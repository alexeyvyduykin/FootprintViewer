using FootprintViewer.ViewModels.TimelinePanel;
using System;

namespace FootprintViewer.Designer;

public class DesignTimeTimelinePanelViewModel : TimelinePanelViewModel
{
    public DesignTimeTimelinePanelViewModel() : base(new DesignTimeData())
    {
        Init.Execute().Subscribe();
    }
}
