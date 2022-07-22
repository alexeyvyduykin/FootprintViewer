using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeUserGeometryTab : UserGeometryTab
    {
        public DesignTimeUserGeometryTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
