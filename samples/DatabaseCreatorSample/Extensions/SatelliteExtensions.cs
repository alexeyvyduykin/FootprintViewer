using DatabaseCreatorSample.Data;
using DatabaseCreatorSample.Science;
using System;

namespace DatabaseCreatorSample;

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

    public static PRDCTSensor ToPRDCTSensor(this Satellite satellite, SwathDirection direction)
    {
        var inner = satellite.InnerHalfAngleDeg;
        var outer = satellite.OuterHalfAngleDeg;
        var angle = (outer - inner) / 2.0;
        var roll = inner + angle;

        return direction switch
        {
            SwathDirection.Left => new PRDCTSensor(angle, roll),
            SwathDirection.Right => new PRDCTSensor(angle, -roll),
            _ => throw new Exception(),
        };
    }
}
