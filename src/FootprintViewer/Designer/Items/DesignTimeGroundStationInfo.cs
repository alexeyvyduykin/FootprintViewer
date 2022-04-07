using FootprintViewer.Data;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundStationInfo : GroundStationInfo
    {
        public DesignTimeGroundStationInfo() : base(BuildModel())
        {
            IsShow = true;
        }

        public static GroundStation BuildModel()
        {
            return new GroundStation()
            {
                Name = $"London",
                Center = new NetTopologySuite.Geometries.Point(-0.118092, 51.509865),
                Angles = new double[] { 12, 18, 22, 26, 30 },
            };
        }
    }
}
