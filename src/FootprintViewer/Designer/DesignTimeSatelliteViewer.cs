using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeSatelliteViewer : SatelliteViewer
    {
        public DesignTimeSatelliteViewer() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
