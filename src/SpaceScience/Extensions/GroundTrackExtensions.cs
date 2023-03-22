using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class GroundTrackExtensions
{
    public static List<(double lonDeg, double latDeg)> GetTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

        if (lonConverter != null)
        {
            return track.CacheTrack
                .Select(s => (lonConverter.Invoke(s.lonDeg + offset), s.latDeg))
                .ToList();
        }

        return track.CacheTrack
            .Select(s => (s.lonDeg + offset, s.latDeg))
            .ToList();
    }

    public static List<List<(double lonDeg, double latDeg)>> GetCutTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
    {
        var res = new List<List<(double, double)>>();

        var list = track.GetTrack(node, lonConverter);

        var temp = new List<(double, double)>();

        var (prevLonDeg, prevLatDeg) = list.FirstOrDefault();

        foreach (var (curLonDeg, curLatDeg) in list)
        {
            if (Math.Abs((curLonDeg - prevLonDeg) * SpaceMath.DegreesToRadians) >= 3.2)
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
