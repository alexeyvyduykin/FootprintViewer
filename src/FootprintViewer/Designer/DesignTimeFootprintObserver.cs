using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintObserver : FootprintObserver
    {
        public DesignTimeFootprintObserver() : base(new DesignTimeData())
        {
            var provider = new DesignDataFootprintProvider();

            UpdateAsync(provider.GetFootprints);
        }
    }
}
