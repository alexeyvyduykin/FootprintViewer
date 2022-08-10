using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintTab : FootprintTab
    {
        public DesignTimeFootprintTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
