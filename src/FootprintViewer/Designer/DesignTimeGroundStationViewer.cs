using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundStationViewer : GroundStationViewer
    {
        public DesignTimeGroundStationViewer() : base(new DesignTimeData())
        {
            IsActive = true;
        }
    }
}
