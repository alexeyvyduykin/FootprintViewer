using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintPreviewTab : FootprintPreviewTabViewModel
    {
        public DesignTimeFootprintPreviewTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
