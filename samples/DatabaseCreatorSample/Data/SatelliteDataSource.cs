using System;
using System.Collections.Generic;

namespace DatabaseCreatorSample.Data
{
    public static class SatelliteDataSource
    {
        private static readonly IList<Satellite> _satellites;

        static SatelliteDataSource()
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

            //var orbit1 = new Orbit(6945.03, 0.0, 97.65 * ScienceMath.DegreesToRadians, 0.0, 0.0 * ScienceMath.DegreesToRadians, raan1 * ScienceMath.DegreesToRadians, 5760.0, dt);
            //var orbit2 = new Orbit(6945.03, 0.0, 97.65 * ScienceMath.DegreesToRadians, 0.0, 72.0 * ScienceMath.DegreesToRadians, raan2 * ScienceMath.DegreesToRadians, 5760.0, dt);
            //var orbit3 = new Orbit(6945.03, 0.0, 97.65 * ScienceMath.DegreesToRadians, 0.0, 144.0 * ScienceMath.DegreesToRadians, raan3 * ScienceMath.DegreesToRadians, 5760.0, dt);
            //var orbit4 = new Orbit(6945.03, 0.0, 97.65 * ScienceMath.DegreesToRadians, 0.0, 216.0 * ScienceMath.DegreesToRadians, raan4 * ScienceMath.DegreesToRadians, 5760.0, dt);
            //var orbit5 = new Orbit(6945.03, 0.0, 97.65 * ScienceMath.DegreesToRadians, 0.0, 288.0 * ScienceMath.DegreesToRadians, raan5 * ScienceMath.DegreesToRadians, 5760.0, dt);
            //// double a, double ecc, double incl, double argOfPer, double lonAN, double om, double period, DateTime epoch

            _satellites = new List<Satellite>()
            {
                new Satellite(){ Name = "Satellite1", Semiaxis = 6945.03, Eccentricity = 0.0, Inclination = 97.65, ArgumentOfPerigee = 0.0, LonAN =   0.0, RAAN = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngle = 32, OuterHalfAngle = 48 },
                new Satellite(){ Name = "Satellite2", Semiaxis = 6945.03, Eccentricity = 0.0, Inclination = 97.65, ArgumentOfPerigee = 0.0, LonAN =  72.0, RAAN = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngle = 32, OuterHalfAngle = 48 },
                new Satellite(){ Name = "Satellite3", Semiaxis = 6945.03, Eccentricity = 0.0, Inclination = 97.65, ArgumentOfPerigee = 0.0, LonAN = 144.0, RAAN = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngle = 32, OuterHalfAngle = 48 },
                new Satellite(){ Name = "Satellite4", Semiaxis = 6945.03, Eccentricity = 0.0, Inclination = 97.65, ArgumentOfPerigee = 0.0, LonAN = 216.0, RAAN = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngle = 32, OuterHalfAngle = 48 },
                new Satellite(){ Name = "Satellite5", Semiaxis = 6945.03, Eccentricity = 0.0, Inclination = 97.65, ArgumentOfPerigee = 0.0, LonAN = 288.0, RAAN = raan1, Period = 5760.0, Epoch = dt, InnerHalfAngle = 32, OuterHalfAngle = 48 },
            };
        }

        private static double GetRAAN(DateTime epoch, double tAN, double lonAN)
        {
            Julian jd = new Julian(epoch);
            double S = jd.ToGmst();
            //double S = orbitState.SiderealTime();       
            return (tAN * Constants.Omega + S) * ScienceMath.RadiansToDegrees + lonAN;
        }

        public static IEnumerable<Satellite> Satellites => _satellites;
    }
}
