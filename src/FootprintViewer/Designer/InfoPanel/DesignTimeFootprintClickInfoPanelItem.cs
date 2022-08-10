using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintClickInfoPanelItem : FootprintClickInfoPanel
    {
        public DesignTimeFootprintClickInfoPanelItem() : base(new FootprintViewModel(DesignTimeFootprintViewModel.BuildModel()))
        {

        }
    }
}
