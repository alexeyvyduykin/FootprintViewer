using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer;

public class DesignTimeFootprintTab : FootprintTabViewModel
{
    public DesignTimeFootprintTab() : base(new DesignTimeData())
    {
        SearchString = "footprint";

        ((FootprintTabFilterViewModel)Filter).FromNode = 2;

        IsActive = true;
    }
}
