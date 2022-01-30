using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeFootprintInfo : FootprintInfo
    {
        public DesignTimeFootprintInfo() : base(BuildModel())
        {
            IsShowInfo = true;
        }

        private static Footprint BuildModel()
        {
            return new Footprint()
            {
                Name = "Footrpint001",
                SatelliteName = "Satellite1",
                Center = new Point(54.434545, -12.435454),
                Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                Duration = 35,
                Node = 11,
                Direction = SatelliteStripDirection.Left,
            };
        }
    }
}
