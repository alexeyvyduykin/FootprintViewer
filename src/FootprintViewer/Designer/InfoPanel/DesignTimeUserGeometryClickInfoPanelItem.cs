using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryClickInfoPanelItem : UserGeometryClickInfoPanel
    {
        public DesignTimeUserGeometryClickInfoPanelItem() : base(new UserGeometryViewModel(DesignTimeUserGeometryViewModel.BuildModel()))
        {

        }
    }
}
