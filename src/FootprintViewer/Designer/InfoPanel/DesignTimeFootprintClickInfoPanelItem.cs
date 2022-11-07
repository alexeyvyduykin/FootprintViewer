using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel.Items;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintClickInfoPanelItem : FootprintClickInfoPanel
    {
        public DesignTimeFootprintClickInfoPanelItem() : base(new FootprintViewModel(DesignTimeFootprintViewModel.BuildModel()))
        {

        }
    }
}
