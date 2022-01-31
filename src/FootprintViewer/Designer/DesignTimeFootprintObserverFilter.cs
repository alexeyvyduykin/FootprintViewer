using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintObserverFilter : FootprintObserverFilter
    {
        public DesignTimeFootprintObserverFilter() : base(new DesignTimeData()) { }
    }
}
