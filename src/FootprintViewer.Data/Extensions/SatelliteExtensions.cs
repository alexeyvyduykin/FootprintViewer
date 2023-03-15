using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Model;

namespace FootprintViewer.Data.Extensions;

public static class SatelliteExtensions
{
    public static PRDCTSatellite ToPRDCTSatellite(this Satellite satellite)
    {
        var a = satellite.Semiaxis;
        var ecc = satellite.Eccentricity;
        var incl = satellite.InclinationDeg * SpaceMath.DegreesToRadians;
        var argOfPer = satellite.ArgumentOfPerigeeDeg * SpaceMath.DegreesToRadians;
        var lonAN = satellite.LongitudeAscendingNodeDeg * SpaceMath.DegreesToRadians;
        var raan = satellite.RightAscensionAscendingNodeDeg * SpaceMath.DegreesToRadians;
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
