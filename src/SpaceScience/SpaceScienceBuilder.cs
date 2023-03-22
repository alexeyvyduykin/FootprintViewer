using SpaceScience.Model;

namespace SpaceScience;

public class GroundStationBuilderResult
{
    public (double lon, double lat) Center { get; init; } = new();

    public double InnerAngle { get; init; }

    public double OuterAngle { get; init; }

    public List<IEnumerable<IEnumerable<(double lon, double lat)>>> Areas { get; init; } = new();

    public List<IEnumerable<(double lon, double lat)>> InnerBorder { get; init; } = new();

    public List<IEnumerable<(double lon, double lat)>> OuterBorder { get; init; } = new();
}

public static class SpaceScienceBuilder
{
    private static readonly double[] _defaultAngles = new[] { 0.0, 10.0 };

    public static GroundStationBuilderResult BuildGroundStation(double lon, double lat, double[] angles)
    {
        var (isHole, anglesRes) = Verify(angles);
        var circles = EarthGeometry.BuildCircles(lon, lat, anglesRes);

        return new GroundStationBuilderResult()
        {
            Center = (lon, lat),
            InnerAngle = (isHole == false) ? 0.0 : anglesRes.First(),
            OuterAngle = anglesRes.Last(),
            Areas = circles.Select(s => s.Areas).ToList(),
            InnerBorder = (isHole == false) ? new List<IEnumerable<(double, double)>>() : circles.First().Borders.ToList(),
            OuterBorder = circles.Last().Borders.ToList(),
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