using SpaceScience.Model;

namespace SpaceScience;

public class TrackBuilderResult
{
    private readonly Dictionary<int, List<List<(double, double)>>> _tracks;

    internal TrackBuilderResult(Dictionary<int, List<List<(double, double)>>> tracks)
    {
        _tracks = tracks;
    }

    public Dictionary<int, List<List<(double, double)>>> Track => _tracks;
}

public class SwathBuilderResult
{
    private readonly Dictionary<int, List<List<(double, double)>>> _leftSwaths;
    private readonly Dictionary<int, List<List<(double, double)>>> _rightSwaths;

    internal SwathBuilderResult(Dictionary<int, List<List<(double, double)>>> leftSwaths, Dictionary<int, List<List<(double, double)>>> rightSwaths)
    {
        _leftSwaths = leftSwaths;
        _rightSwaths = rightSwaths;
    }

    public Dictionary<int, List<List<(double, double)>>> Left => _leftSwaths;

    public Dictionary<int, List<List<(double, double)>>> Right => _rightSwaths;
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

    public static SwathBuilderResult BuildSwaths(PRDCTSatellite satellite, double verticalHalfAngleDEG, double rollAngleDEG)
    {
        var leftSwath = new Swath(satellite.Orbit, verticalHalfAngleDEG, Math.Abs(rollAngleDEG));
        var rightSwath = new Swath(satellite.Orbit, verticalHalfAngleDEG, -Math.Abs(rollAngleDEG));

        var leftRes = BuildSwaths(satellite, leftSwath);
        var rightRes = BuildSwaths(satellite, rightSwath);

        return new SwathBuilderResult(leftRes, rightRes);
    }

    private static Dictionary<int, List<List<(double lon, double lat)>>> BuildSwaths(PRDCTSatellite satellite, Swath swath)
    {
        var swaths = new Dictionary<int, List<List<(double, double)>>>();

        foreach (var node in satellite.Nodes().Select(s => s.Value))
        {
            var near = swath.GetNearGroundTrack(satellite, node - 1, SpaceConverters.From180To180);
            var far = swath.GetFarGroundTrack(satellite, node - 1, SpaceConverters.From180To180);

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => g.Deconstruct()).ToList()).ToList();

            swaths.Add(node, convertShapes);
        }

        return swaths;
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
