using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeSatelliteTab : SatelliteTab
    {
        public DesignTimeSatelliteTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
