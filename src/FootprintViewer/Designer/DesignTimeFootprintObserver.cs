using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintObserver : FootprintObserver
    {
        public DesignTimeFootprintObserver() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
