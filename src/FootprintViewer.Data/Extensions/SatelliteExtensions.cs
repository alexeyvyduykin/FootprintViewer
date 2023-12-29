using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using static SpaceScience.Extensions.OrbitExtensions;

namespace FootprintViewer.Data.Extensions;

public static class SatelliteExtensions
{
    public static Orbit ToOrbit(this Satellite satellite)
    {
        var a = satellite.Semiaxis;
        var ecc = satellite.Eccentricity;
        var inclDeg = satellite.InclinationDeg;
        var argOfPerDeg = satellite.ArgumentOfPerigeeDeg;
        var lonANDeg = satellite.LongitudeAscendingNodeDeg;
        var raanDeg = satellite.RightAscensionAscendingNodeDeg;
        var period = satellite.Period;
        var epoch = satellite.Epoch;

        var factory = new SpaceScienceFactory();

        var orbit = factory.CreateOrbit(a, ecc, inclDeg, argOfPerDeg, lonANDeg, raanDeg, period, epoch);

        return orbit;
    }

    public static int NodesOnDay(this Satellite satellite)
    {
        return satellite.ToOrbit().NodesOnDay();
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildSwaths(this Satellite satellite, Models.SwathDirection direction)
    {
        var orbit = satellite.ToOrbit();

        var dir = direction switch
        {
            Models.SwathDirection.Left => SpaceScience.Model.SwathDirection.Left,
            Models.SwathDirection.Right => SpaceScience.Model.SwathDirection.Right,
            _ => throw new NotImplementedException(),
        };

        var swath = new Swath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, dir);
        var res = swath.BuildSwaths(orbit.ToNodesOnDay());
        return res;
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildTracks(this Satellite satellite)
    {
        var res = satellite.ToOrbit().BuildTracks();

        return res.Track;
    }
}
