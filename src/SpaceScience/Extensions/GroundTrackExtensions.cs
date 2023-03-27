using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class GroundTrackExtensions
{
    public static List<(double lonDeg, double latDeg, double u, double t)> GetFullTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

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

    public static List<(double lonDeg, double latDeg)> GetTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        return track
            .GetFullTrack(node, lonConverter)
            .Select(s => (s.lonDeg, s.latDeg))
            .ToList();
    }

    public static (double lonDeg, double latDeg) GetTrackOfIndex(this GroundTrack track, int index, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

        var (lonDeg, latDeg, _, _) = track.CacheTrack[index];

        if (lonConverter != null)
        {
            return (lonConverter.Invoke(lonDeg + offset), latDeg);
        }

        return (lonDeg + offset, latDeg);
    }

    public static (double lonDeg, double latDeg, double u, double t) GetFullTrackOfIndex(this GroundTrack track, int index, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

        var (lonDeg, latDeg, u, t) = track.CacheTrack[index];

        if (lonConverter != null)
        {
            return (lonConverter.Invoke(lonDeg + offset), latDeg, u, t);
        }

        return (lonDeg + offset, latDeg, u, t);
    }

    // TODO: only for interval (-180;+180) ?
    public static List<List<(double lonDeg, double latDeg)>> ToCutList(this List<(double lonDeg, double latDeg)> list)
    {
        var res = new List<List<(double, double)>>();

        var temp = new List<(double, double)>();

        var (prevLonDeg, prevLatDeg) = list.FirstOrDefault();

        foreach (var (curLonDeg, curLatDeg) in list)
        {
            if (Math.Abs(curLonDeg - prevLonDeg) > 180)
            {
                var cutLatDeg = LinearInterpDiscontLat(prevLonDeg, prevLatDeg, curLonDeg, curLatDeg);

                if (prevLonDeg > 0.0)
                {
                    temp.Add((180.0, cutLatDeg));
                    res.Add(temp);
                    temp = new() { (-180.0, cutLatDeg), (curLonDeg, curLatDeg) };
                }
                else
                {
                    temp.Add((-180.0, cutLatDeg));
                    res.Add(temp);
                    temp = new() { (180.0, cutLatDeg), (curLonDeg, curLatDeg) };
                }
            }
            else
            {
                temp.Add((curLonDeg, curLatDeg));
            }

            prevLonDeg = curLonDeg;
            prevLatDeg = curLatDeg;
        }

        res.Add(temp);

        return res;
    }

    private static double LinearInterpDiscontLat(double lonDeg1, double latDeg1, double lonDeg2, double latDeg2)
    {
        if (lonDeg1 > lonDeg2)
        {
            lonDeg2 += 360.0;
        }
        else
        {
            lonDeg1 += 360.0;
        }

        return (latDeg1 + (180.0 - lonDeg1) * (latDeg2 - latDeg1) / (lonDeg2 - lonDeg1));
    }
}
