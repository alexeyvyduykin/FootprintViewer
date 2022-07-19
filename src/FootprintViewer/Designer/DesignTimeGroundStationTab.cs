using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundStationTab : GroundStationTab
    {
        public DesignTimeGroundStationTab() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
