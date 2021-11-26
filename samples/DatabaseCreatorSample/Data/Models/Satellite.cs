using System;
using System.Collections.Generic;

namespace DatabaseCreatorSample.Data
{
    public class Satellite
    {
        public string Name { get; set; }
        public double Semiaxis { get; set; }
        public double Eccentricity { get; set; }
        public double Inclination { get; set; } 
        public double ArgumentOfPerigee { get; set; }
        public double LonAN { get; set; }
        public double RAAN { get; set; }
        public double Period { get; set; }
        public DateTime Epoch { get; set; }     
        public double InnerHalfAngle { get; set; }
        public double OuterHalfAngle { get; set; }
    }

    public static class SatelliteExtensions
    {
        public static PRDCTSatellite ToPRDCTSatellite(this Satellite satellite)
        {
            var a = satellite.Semiaxis;
            var ecc = satellite.Eccentricity;
            var incl = satellite.Inclination * ScienceMath.DegreesToRadians;
            var argOfPer = satellite.ArgumentOfPerigee * ScienceMath.DegreesToRadians;
            var lonAN = satellite.LonAN * ScienceMath.DegreesToRadians;
            var raan = satellite.RAAN * ScienceMath.DegreesToRadians;
            var period = satellite.Period;
            var epoch = satellite.Epoch;

            var orbit = new Orbit(a, ecc, incl, argOfPer, lonAN, raan, period, epoch);
            
            return new PRDCTSatellite(orbit, 1);
        }

        public static PRDCTSensor ToPRDCTSensor(this Satellite satellite, string direction)
        {
            var inner = satellite.InnerHalfAngle;
            var outer = satellite.OuterHalfAngle;
            var angle = (outer - inner) / 2.0;
            var roll = inner + angle;

            if (direction == "Left")
            {
                return new PRDCTSensor(angle, roll);
            }
            else if(direction == "Right")
            {
                return new PRDCTSensor(angle, -roll);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
