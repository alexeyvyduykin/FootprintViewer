using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class SwathExtensions
{
    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath swath, int node, double duration, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, duration, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath swath, int node, double duration, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, duration, lonConverter);
    }
}
