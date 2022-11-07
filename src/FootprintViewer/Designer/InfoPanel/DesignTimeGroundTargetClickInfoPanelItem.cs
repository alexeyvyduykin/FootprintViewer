using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel.Items;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetClickInfoPanelItem : GroundTargetClickInfoPanel
    {
        public DesignTimeGroundTargetClickInfoPanelItem() : base(new GroundTargetViewModel(DesignTimeGroundTargetViewModel.BuildModel()))
        {

        }
    }
}
