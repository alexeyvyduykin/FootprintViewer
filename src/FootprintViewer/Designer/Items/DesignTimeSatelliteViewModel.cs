using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeSatelliteViewModel : SatelliteViewModel
    {
        private readonly static Random _random = new Random();

        public DesignTimeSatelliteViewModel() : base(BuildModel())
        {
            IsShow = true;
            IsShowInfo = true;
        }

        public static Satellite BuildModel()
        {
            return new Satellite()
            {
                Name = $"Satellite{_random.Next(1, 10):00}",
                Semiaxis = 6945.03,
                Eccentricity = 0.0,
                InclinationDeg = 97.65,
                ArgumentOfPerigeeDeg = 0.0,
                LongitudeAscendingNodeDeg = 0.0,
                RightAscensionAscendingNodeDeg = 0.0,
                Period = 5760.0,
                Epoch = DateTime.Now,
                InnerHalfAngleDeg = 32,
                OuterHalfAngleDeg = 48
            };
        }
    }
}
