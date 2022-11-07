using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel.Items;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryClickInfoPanelItem : UserGeometryClickInfoPanel
    {
        public DesignTimeUserGeometryClickInfoPanelItem() : base(new UserGeometryViewModel(DesignTimeUserGeometryViewModel.BuildModel()))
        {

        }
    }
}
