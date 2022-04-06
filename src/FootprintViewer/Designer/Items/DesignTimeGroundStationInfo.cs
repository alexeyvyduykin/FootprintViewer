using FootprintViewer.Data;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundStationInfo : GroundStationInfo
    {
        public DesignTimeGroundStationInfo() : base(BuildModel())
        {

        }

        public static GroundStation BuildModel()
        {
            return new GroundStation()
            {
                Name = $"London",
                Center = new NetTopologySuite.Geometries.Point(-0.118092, 51.509865),
            };
        }
    }
}
