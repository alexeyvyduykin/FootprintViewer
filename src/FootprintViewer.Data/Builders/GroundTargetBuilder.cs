using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SpaceScience;
using SpaceScience.Model;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class GroundTargetBuilder
{
    private static readonly Random _random = new();

    public static async Task<IList<GroundTarget>> CreateAsync(IList<Footprint> footprints, int count)
    => await Observable.Start(() => Create(footprints, count));

    public static async Task<IList<GroundTarget>> CreateAsync(int count)
        => await Observable.Start(() => Create(count));

    public static GroundTarget CreateRandom()
    {
        var type = (GroundTargetType)_random.Next(0, 3);

        return new GroundTarget()
        {
            Name = $"GroundTarget{_random.Next(1, 101):000}",
            Type = type,
        };
    }

    public static IList<GroundTarget> Create(IList<Footprint> footprints, int targetCount)
    {
        var targets = new List<GroundTarget>();

        int count1 = footprints.Count;

        int index = 0;

        for (int i = 0; i < count1; i++)
        {
            var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

            var targetName = footprints[i].TargetName ?? $"EmptyGroundTarget{++index:0000}";

            var target = CreateRandomTarget(targetName, type, footprints[i].Center!);

            targets.Add(target);
        }

        var count2 = targetCount - count1;

        if (count2 > 0)
        {
            for (int i = 0; i < count2; i++)
            {
                var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

                var center = new Point(_random.Next(-180, 180 + 1), _random.Next(-80, 80 + 1));

                var target = CreateRandomTarget($"EmptyGroundTarget{++index:0000}", type, center);

                targets.Add(target);
            }
        }

        return targets;
    }

    public static IList<GroundTarget> Create(int targetCount)
    {
        var targets = new List<GroundTarget>();

        for (int i = 0; i < targetCount; i++)
        {
            var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

            var center = new Point(_random.Next(-180, 180 + 1), _random.Next(-80, 80 + 1));

            var name = $"GroundTarget{(i + 1):0000}";

            var target = CreateRandomTarget(name, type, center);

            targets.Add(target);
        }

        return targets;
    }

    private static GroundTarget CreateRandomTarget(string name, GroundTargetType type, Point center)
    {
        return type switch
        {
            GroundTargetType.Point =>
            new GroundTarget()
            {
                Name = name,
                Type = GroundTargetType.Point,
                Points = new Point(center.X, center.Y),
            },
            GroundTargetType.Route => new GroundTarget()
            {
                Name = name,
                Type = GroundTargetType.Route,
                Points = CreateRoute(center)
            },
            GroundTargetType.Area => new GroundTarget()
            {
                Name = name,
                Type = GroundTargetType.Area,
                Points = CreateArea(center)
            },
            _ => throw new Exception()
        };
    }

    private static Geometry CreateRoute(Point center)
    {
        var list = new List<Coordinate>();
        double r = _random.Next(10, 20 + 1) / 10.0;
        double d = 2 * r;

        var lon0 = center.X - r;
        var lon1 = center.X + r;

        lon0 = LonConverters.Default(lon0);
        lon1 = LonConverters.Default(lon1);

        var begin = new Coordinate(lon0, center.Y);
        var end = new Coordinate(lon1, center.Y);

        var count = _random.Next(2, 5 + 1);
        var dd = d / (count + 1);

        var last = begin;

        list.Add(begin);

        for (int i = 0; i < count; i++)
        {
            var directionAngle = (_random.Next(0, 120 + 1) - 60) * SpaceMath.DegreesToRadians;
            var yd = dd * Math.Tan(directionAngle);
            var lon = last.X + dd;

            lon = LonConverters.Default(lon);

            var point = new Coordinate(lon, last.Y + yd);

            list.Add(point);

            last = point;
        }

        list.Add(end);

        return new LineString(list.ToArray());
    }

    private static Geometry CreateArea(Point center)
    {
        var list = new List<Coordinate>();

        var angle0 = (double)_random.Next(0, 90 + 1);
        var vertexCount = _random.Next(4, 10 + 1);
        var angleDelta = 360.0 / vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            var r = _random.Next(2, 10) / 10.0;
            var a = angle0 * SpaceMath.DegreesToRadians;

            var (dlon, dlat) = (r * Math.Cos(a), r * Math.Sin(a));

            var lon = center.X + dlon;

            lon = LonConverters.Default(lon);

            var point = new Coordinate(lon, center.Y + dlat);

            list.Add(point);

            angle0 -= angleDelta;
        }

        return new LineString(list.ToArray());
    }
}
