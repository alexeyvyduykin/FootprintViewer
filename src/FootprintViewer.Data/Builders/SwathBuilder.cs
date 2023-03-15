using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Model;

namespace FootprintViewer.Data.Builders;

public static class SwathBuilder
{
    public static Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> CreateLeft(IEnumerable<Satellite> satellites)
    {
        var leftSwaths = new Dictionary<string, Dictionary<int, List<List<(double, double)>>>>();

        foreach (var satellite in satellites)
        {
            var sat = satellite.ToPRDCTSatellite();

            var sensorLeft = satellite.ToPRDCTSensor(SwathDirection.Left);

            var swath = new Swath(sat.Orbit, sensorLeft.VerticalHalfAngleDEG, sensorLeft.RollAngleDEG);

            var left = BuildSwaths(sat, swath);

            leftSwaths.Add(satellite.Name!, left);
        }

        return leftSwaths;
    }

    public static Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> CreateRight(IEnumerable<Satellite> satellites)
    {
        var rightSwaths = new Dictionary<string, Dictionary<int, List<List<(double, double)>>>>();

        foreach (var satellite in satellites)
        {
            var sat = satellite.ToPRDCTSatellite();

            var sensorRight = satellite.ToPRDCTSensor(SwathDirection.Right);

            var swath = new Swath(sat.Orbit, sensorRight.VerticalHalfAngleDEG, sensorRight.RollAngleDEG);

            var right = BuildSwaths(sat, swath);

            rightSwaths.Add(satellite.Name!, right);
        }

        return rightSwaths;
    }

    private static Dictionary<int, List<List<(double lon, double lat)>>> BuildSwaths(PRDCTSatellite satellite, Swath swath)
    {
        var swaths = new Dictionary<int, List<List<(double, double)>>>();

        foreach (var node in satellite.Nodes().Select(s => s.Value))
        {
            var near = swath.GetNearGroundTrack(satellite, node - 1, SpaceConverters.From180To180).ToList();
            var far = swath.GetFarGroundTrack(satellite, node - 1, SpaceConverters.From180To180).ToList();

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            swaths.Add(node, new List<List<(double, double)>>());

            foreach (var item in shapes)
            {
                swaths[node].Add(new List<(double, double)>(item.Select(s => (s.Lon, s.Lat))));
            }
        }

        return swaths;
    }
}
