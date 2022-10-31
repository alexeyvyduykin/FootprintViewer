using FootprintViewer.Data.Science;
using Newtonsoft.Json;
using System;

namespace FootprintViewer.Data;

[JsonObject]
public class Satellite
{
    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Semiaxis")]
    public double Semiaxis { get; set; }

    [JsonProperty("Eccentricity")]
    public double Eccentricity { get; set; }

    [JsonProperty("InclinationDeg")]
    public double InclinationDeg { get; set; }

    [JsonProperty("ArgumentOfPerigeeDeg")]
    public double ArgumentOfPerigeeDeg { get; set; }

    [JsonProperty("LongitudeAscendingNodeDeg")]
    public double LongitudeAscendingNodeDeg { get; set; }

    [JsonProperty("RightAscensionAscendingNodeDeg")]
    public double RightAscensionAscendingNodeDeg { get; set; }

    [JsonProperty("Period")]
    public double Period { get; set; }

    [JsonProperty("Epoch")]
    public DateTime Epoch { get; set; }

    [JsonProperty("InnerHalfAngleDeg")]
    public double InnerHalfAngleDeg { get; set; }

    [JsonProperty("OuterHalfAngleDeg")]
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
