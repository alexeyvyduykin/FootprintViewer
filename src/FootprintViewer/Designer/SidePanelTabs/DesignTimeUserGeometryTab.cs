using FootprintViewer.ViewModels.SidePanel.Tabs;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryTab : UserGeometryTabViewModel
    {
        public DesignTimeUserGeometryTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
