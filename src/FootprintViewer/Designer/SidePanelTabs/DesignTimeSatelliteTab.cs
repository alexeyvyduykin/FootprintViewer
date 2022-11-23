using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer;

public class DesignTimeSatelliteTab : SatelliteTabViewModel
{
    public DesignTimeSatelliteTab() : base(new DesignTimeData())
    {
        IsActive = true;
    }
}
