using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class BandExtensions
{
    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath22 swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath22 swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, lonConverter);
    }
}
