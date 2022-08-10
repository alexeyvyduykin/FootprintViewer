using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetClickInfoPanelItem : GroundTargetClickInfoPanel
    {
        public DesignTimeGroundTargetClickInfoPanelItem() : base(new GroundTargetViewModel(DesignTimeGroundTargetViewModel.BuildModel()))
        {

        }
    }
}
