using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class GroundTrackExtensions
{
    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static List<(double lonDeg, double latDeg, double u, double t)> GetFullTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        if (node < 1)
        {
            node = 1;
        }

        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * (node - 1);

        if (lonConverter != null)
        {
            return track.CacheTrack
                .Select(s => (lonConverter.Invoke(s.lonDeg + offset), s.latDeg, s.u, s.t))
                .ToList();
        }

        return track.CacheTrack
            .Select(s => (s.lonDeg + offset, s.latDeg, s.u, s.t))
            .ToList();
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static List<(double lonDeg, double latDeg, double u, double t)> GetFullTrack(this GroundTrack track, int node, double duration, Func<double, double>? lonConverter = null)
    {
        if (node < 1)
        {
            node = 1;
        }

        var list = new List<(double lonDeg, double latDeg, double u, double t)>();

        // TODO: duration cut
        double durationSum = 0.0;
        int node1 = node - 1;
        var uPrev = track.CacheTrack.First().u;
        var tPrev = track.CacheTrack.First().t;

        if (lonConverter != null)
        {
            foreach (var item in track.CacheTrack)
            {
                if (item.u < uPrev)
                {
                    node1++;
                }

                var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node1;

                list.Add((lonConverter.Invoke(item.lonDeg + offset), item.latDeg, item.u, item.t));

                uPrev = item.u;
            }

            return list;
        }

        foreach (var item in track.CacheTrack)
        {
            if (item.u < uPrev)
            {
                node1++;
            }

            var dd = item.t - tPrev;

            var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node1;

            list.Add((item.lonDeg + offset, item.latDeg, item.u, item.t));

            uPrev = item.u;
            tPrev = item.t;
        }

        return list;
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static List<(double lonDeg, double latDeg)> GetTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        return track
            .GetFullTrack(node, lonConverter)
            .Select(s => (s.lonDeg, s.latDeg))
            .ToList();
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static List<(double lonDeg, double latDeg)> GetTrack(this GroundTrack track, int node, double duration, Func<double, double>? lonConverter = null)
    {
        if (node < 1)
        {
            node = 1;
        }

        var node1 = node;

        // TODO: remove nodeCorrect
        if (track._isNodeCorrect == true)
        {
            node1--;
        }

        return track
            .GetFullTrack(node1, duration, lonConverter)
            .Select(s => (s.lonDeg, s.latDeg))
            .ToList();
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static (double lonDeg, double latDeg) GetTrackOfIndex(this GroundTrack track, int index, int node, Func<double, double>? lonConverter = null)
    {
        if (node < 1)
        {
            node = 1;
        }

        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * (node - 1);

        var (lonDeg, latDeg, _, _) = track.CacheTrack[index];

        if (lonConverter != null)
        {
            return (lonConverter.Invoke(lonDeg + offset), latDeg);
        }

        return (lonDeg + offset, latDeg);
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static (double lonDeg, double latDeg, double u, double t) GetFullTrackOfIndex(this GroundTrack track, int index, int node, Func<double, double>? lonConverter = null)
    {
        if (node < 1)
        {
            node = 1;
        }

        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * (node - 1);

        var (lonDeg, latDeg, u, t) = track.CacheTrack[index];

        if (lonConverter != null)
        {
            return (lonConverter.Invoke(lonDeg + offset), latDeg, u, t);
        }

        return (lonDeg + offset, latDeg, u, t);
    }
}
