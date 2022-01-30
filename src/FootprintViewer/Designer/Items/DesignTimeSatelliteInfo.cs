using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeSatelliteInfo : SatelliteInfo
    {
        public DesignTimeSatelliteInfo() : base(BuildModel())
        {
            IsShow = true;
            IsShowInfo = true;
        }

        private static Satellite BuildModel()
        {
            return new Satellite()
            {
                Name = "Satellite1",
                Semiaxis = 6945.03,
                Eccentricity = 0.0,
                InclinationDeg = 97.65,
                ArgumentOfPerigeeDeg = 0.0,
                LongitudeAscendingNodeDeg = 0.0,
                RightAscensionAscendingNodeDeg = 0.0,
                Period = 5760.0,
                Epoch = new DateTime(2000, 6, 1, 12, 0, 0),
                InnerHalfAngleDeg = 32,
                OuterHalfAngleDeg = 48
            };
        }
    }
}
