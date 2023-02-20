using NetTopologySuite.Geometries;
using SpaceScience;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data;

internal static class StripBuilder
{
    public static (Dictionary<string, Dictionary<int, List<List<Point>>>> left, Dictionary<string, Dictionary<int, List<List<Point>>>> right) Create(IEnumerable<Satellite> satellites)
    {
        var leftStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();
        var rightStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();

        foreach (var satellite in satellites)
        {
            var sat = satellite.ToPRDCTSatellite();
            var sensorLeft = satellite.ToPRDCTSensor(SwathDirection.Left);
            var sensorRight = satellite.ToPRDCTSensor(SwathDirection.Right);

            var swath1 = new Swath(sat.Orbit, sensorLeft.VerticalHalfAngleDEG, sensorLeft.RollAngleDEG);
            var swath2 = new Swath(sat.Orbit, sensorRight.VerticalHalfAngleDEG, sensorRight.RollAngleDEG);

            var left = BuildStrips(sat, swath1);
            var right = BuildStrips(sat, swath2);

            leftStrips.Add(satellite.Name, left);
            rightStrips.Add(satellite.Name, right);
        }

        return (leftStrips, rightStrips);
    }

    private static Dictionary<int, List<List<Point>>> BuildStrips(PRDCTSatellite satellite, Swath swath)
    {
        var strips = new Dictionary<int, List<List<Point>>>();

        foreach (var node in satellite.Nodes().Select(s => s.Value))
        {
            var near = swath.GetNearGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();
            var far = swath.GetFarGroundTrack(satellite, node - 1, ScienceConverters.From180To180).ToList();

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            strips.Add(node, new List<List<Point>>());

            foreach (var item in shapes)
            {
                strips[node].Add(new List<Point>(item.Select(s => new Point(s.Lon, s.Lat))));
            }
        }

        return strips;
    }
}
