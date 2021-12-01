using FootprintViewer.Data.Science;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class Satellite
    {
        public string Name { get; set; }
        public double Semiaxis { get; set; }
        public double Eccentricity { get; set; }
        public double InclinationDeg { get; set; } 
        public double ArgumentOfPerigeeDeg { get; set; }
        public double LongitudeAscendingNodeDeg { get; set; }
        public double RightAscensionAscendingNodeDeg { get; set; }
        public double Period { get; set; }
        public DateTime Epoch { get; set; }     
        public double InnerHalfAngleDeg { get; set; }
        public double OuterHalfAngleDeg { get; set; }
    }

    public static class SatelliteExtensions
    {
        public static PRDCTSatellite ToPRDCTSatellite(this Satellite satellite)
        {
            var a = satellite.Semiaxis;
            var ecc = satellite.Eccentricity;
            var incl = satellite.InclinationDeg * ScienceMath.DegreesToRadians;
            var argOfPer = satellite.ArgumentOfPerigeeDeg * ScienceMath.DegreesToRadians;
            var lonAN = satellite.LongitudeAscendingNodeDeg * ScienceMath.DegreesToRadians;
            var raan = satellite.RightAscensionAscendingNodeDeg * ScienceMath.DegreesToRadians;
            var period = satellite.Period;
            var epoch = satellite.Epoch;

            var orbit = new Orbit(a, ecc, incl, argOfPer, lonAN, raan, period, epoch);
            
            return new PRDCTSatellite(orbit, 1);
        }

        public static PRDCTSensor ToPRDCTSensor(this Satellite satellite, SatelliteStripDirection direction)
        {
            var inner = satellite.InnerHalfAngleDeg;
            var outer = satellite.OuterHalfAngleDeg;
            var angle = (outer - inner) / 2.0;
            var roll = inner + angle;

            switch (direction)
            {
                case SatelliteStripDirection.Left:
                    return new PRDCTSensor(angle, roll);
                case SatelliteStripDirection.Right:
                    return new PRDCTSensor(angle, -roll);
                default:
                    throw new Exception();
            }
        }
    }
}
