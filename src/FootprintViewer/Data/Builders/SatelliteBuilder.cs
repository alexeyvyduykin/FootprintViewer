using FootprintViewer.Data.Science;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    internal static class SatelliteBuilder
    {
        public static IEnumerable<Satellite> Create()
        {
            var dt = Convert.ToDateTime("1 Jul 2007 12:00:00.000");

            var raan1 = GetRAAN(dt, 0.0, 0.0);
            while (raan1 > 360.0)
            { raan1 -= 360.0; }

            var raan2 = GetRAAN(dt, 0.0, 72.0);
            while (raan2 > 360.0)
            { raan2 -= 360.0; }

            var raan3 = GetRAAN(dt, 0.0, 144.0);
            while (raan3 > 360.0)
            { raan3 -= 360.0; }

            var raan4 = GetRAAN(dt, 0.0, 216.0);
            while (raan4 > 360.0)
            { raan4 -= 360.0; }

            var raan5 = GetRAAN(dt, 0.0, 288.0);
            while (raan5 > 360.0)
            { raan5 -= 360.0; }

            // double a, double ecc, double incl, double argOfPer, double lonAN, double om, double period, DateTime epoch
            return new List<Satellite>()
            {
                new Satellite(){ Name = "Satellite1", Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg =   0.0, RightAscensionAscendingNodeDeg = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 },
                new Satellite(){ Name = "Satellite2", Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg =  72.0, RightAscensionAscendingNodeDeg = raan2, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 },
                new Satellite(){ Name = "Satellite3", Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg = 144.0, RightAscensionAscendingNodeDeg = raan3, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 },
                new Satellite(){ Name = "Satellite4", Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg = 216.0, RightAscensionAscendingNodeDeg = raan4, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 },
                new Satellite(){ Name = "Satellite5", Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg = 288.0, RightAscensionAscendingNodeDeg = raan5, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 },
            };
        }

        private static double GetRAAN(DateTime epoch, double tAN, double lonAN)
        {
            var jd = new Julian(epoch);
            double S = jd.ToGmst();
            //double S = orbitState.SiderealTime();       
            return (tAN * Constants.Omega + S) * ScienceMath.RadiansToDegrees + lonAN;
        }
    }
}
