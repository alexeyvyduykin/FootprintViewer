using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using System.Collections.Generic;

namespace FootprintViewer.Designer;

public class DesignTimeSidePanel : SidePanelViewModel
{
    public DesignTimeSidePanel() : base()
    {
        var data = new DesignTimeData();

        var tabs = new SidePanelTabViewModel[]
        {
            new FootprintPreviewTabViewModel(data),
            new SatelliteTabViewModel(data),
            new GroundStationTabViewModel(data),
            new GroundTargetTabViewModel(data),
            new FootprintTabViewModel(data),
            new UserGeometryTabViewModel(data),
        };

        Tabs.AddRange(new List<SidePanelTabViewModel>(tabs));
    }
}
