using SpaceScience;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data;

internal class GroundStationResult
{
    public (double lon, double lat) Center { get; init; } = new();

    public double InnerAngle { get; init; }

    public double OuterAngle { get; init; }

    public IList<IEnumerable<IEnumerable<(double lon, double lat)>>> Areas { get; init; } = new List<IEnumerable<IEnumerable<(double, double)>>>();

    public IEnumerable<IEnumerable<(double lon, double lat)>> InnerBorder { get; init; } = new List<IEnumerable<(double, double)>>();

    public IEnumerable<IEnumerable<(double lon, double lat)>> OuterBorder { get; init; } = new List<IEnumerable<(double, double)>>();
}

internal static class GroundStationBuilder
{
    private static readonly double[] _defaultAngles = new[] { 0.0, 10.0 };

    public static IList<GroundStationResult> Create(IEnumerable<GroundStation> groundStations)
    {
        return groundStations.Select(s => Create(s)).ToList();
    }

    public static GroundStationResult Create(GroundStation groundStation)
    {
        var lon = groundStation.Center.X;
        var lat = groundStation.Center.Y;
        var (isHole, angles) = Verify(groundStation.Angles);
        var circles = EarthGeometry.BuildCircles(lon, lat, angles);

        return new GroundStationResult()
        {
            Center = (lon, lat),
            InnerAngle = (isHole == false) ? 0.0 : angles.First(),
            OuterAngle = angles.Last(),
            Areas = circles.Select(s => s.Areas).ToList(),
            InnerBorder = (isHole == false) ? new List<IEnumerable<(double, double)>>() : circles.First().Borders,
            OuterBorder = circles.Last().Borders,
        };
    }

    private static (bool isHole, double[] angles) Verify(double[] anglesSource)
    {
        var list = new List<double>(anglesSource);

        list.Sort();

        if (list.Count == 1)
        {
            list.Insert(0, 0.0);
        }

        if (list.Where(s => s != 0.0).Any() == false)
        {
            list.Clear();
            list.AddRange(_defaultAngles);
        }

        var first = list.First();

        var isHole = (first != 0.0);

        var angles = list.Where(s => s != 0.0).ToArray();

        return (isHole, angles);
    }
}
