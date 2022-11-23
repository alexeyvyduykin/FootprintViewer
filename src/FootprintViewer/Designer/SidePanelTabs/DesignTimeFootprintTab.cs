using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer;

public class DesignTimeFootprintTab : FootprintTabViewModel
{
    public DesignTimeFootprintTab() : base(new DesignTimeData())
    {
        SearchString = "footprint";

        IsActive = true;
    }
}
