using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer;

public class DesignTimeGroundStationTab : GroundStationTabViewModel
{
    public DesignTimeGroundStationTab() : base(new DesignTimeData())
    {
        IsActive = true;
    }
}
