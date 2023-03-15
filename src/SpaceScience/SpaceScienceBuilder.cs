using SpaceScience.Model;

namespace SpaceScience;

public class TrackBuilderResult
{
    private readonly Dictionary<int, List<List<(double, double)>>> _tracks;

    internal TrackBuilderResult(Dictionary<int, List<List<(double, double)>>> tracks)
    {
        _tracks = tracks;
    }


}

public static class SpaceScienceBuilder
{

    public static TrackBuilderResult BuildTracks(PRDCTSatellite satellite, double stepSec = 60.0)
    {
        var tracks = new Dictionary<int, List<List<(double, double)>>>();

        foreach (var node in satellite.Nodes())
        {
            var coords = satellite.GetGroundTrackDynStep(node.Value - 1, stepSec, SpaceConverters.From180To180);

            tracks.Add(node.Value, new List<List<(double, double)>>());

            var temp = new List<(double, double)>();

            var prev = coords.FirstOrDefault();
                   
            foreach (var cur in coords)
            {
                var (curLonDeg, curLatDeg) = cur.ToDegrees();

                if (Math.Abs(cur.Lon - prev!.Lon) >= 3.2)
                {
                    var cutLatDeg = LinearInterpDiscontLat(prev, cur) * SpaceMath.RadiansToDegrees;

                    if (prev.Lon > 0.0)
                    {
                        temp.Add((180.0, cutLatDeg));
                        tracks[node.Value].Add(temp);
                        temp = new()
                        {
                            (-180.0, cutLatDeg), (curLonDeg, curLatDeg)
                        };
                    }
                    else
                    {
                        temp.Add((-180.0, cutLatDeg));
                        tracks[node.Value].Add(temp);
                        temp = new()
                        {
                            (180.0, cutLatDeg), (curLonDeg, curLatDeg)
                        };
                    }
                }
                else
                {
                    temp.Add((curLonDeg, curLatDeg));
                }

                prev = cur;
            }

            tracks[node.Value].Add(temp);
        }

        return new TrackBuilderResult(tracks);
    }



    private static double LinearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
    {
        // one longitude should be negative one positive, make them both positive
        var (lon1, lat1) = pp1.ToRadians();
        var (lon2, lat2) = pp2.ToRadians();

        if (lon1 > lon2)
        {
            lon2 += 2 * Math.PI; // in radians
        }
        else
        {
            lon1 += 2 * Math.PI;
        }

        return (lat1 + (Math.PI - lon1) * (lat2 - lat1) / (lon2 - lon1));
    }
}
