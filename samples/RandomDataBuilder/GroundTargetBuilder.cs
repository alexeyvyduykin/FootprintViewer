using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using SpaceScience;

namespace RandomDataBuilder;

public static class GroundTargetBuilder
{
    private static readonly Random _random = new();
    private static readonly int _countTargets = 5000;

    public static IEnumerable<GroundTarget> Create(IEnumerable<Footprint>? footprints)
    {
        var targets = new List<GroundTarget>();

        if (footprints == null)
        {
            return targets;
        }

        var list = new List<Footprint>(footprints);

        int counts = list.Count;

        int index = 0;

        for (int i = 0; i < counts; i++)
        {
            var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

            var target = CreateRandomTarget($"GroundTarget{++index:0000}", type, list[i].Center!);

            list[i].TargetName = target.Name;

            targets.Add(target);
        }

        for (int i = 0; i < _countTargets - counts; i++)
        {
            var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

            var center = new Point(_random.Next(-180, 180 + 1), _random.Next(-80, 80 + 1));

            var target = CreateRandomTarget($"GroundTarget{++index:0000}", type, center);

            targets.Add(target);
        }

        return targets;
    }

    private static GroundTarget CreateRandomTarget(string name, GroundTargetType type, Point center)
    {
        switch (type)
        {
            case GroundTargetType.Point:
                return new GroundTarget()
                {
                    Name = name,
                    Type = GroundTargetType.Point,
                    Points = new Point(center.X, center.Y),
                };
            case GroundTargetType.Route:
                {
                    var list = new List<Coordinate>();
                    double r = _random.Next(10, 20 + 1) / 10.0;
                    double d = 2 * r;

                    var lon0 = center.X - r;
                    if (lon0 < -180)
                    {
                        lon0 += 360;
                    }

                    if (lon0 > 180)
                    {
                        lon0 -= 360;
                    }

                    var begin = new Coordinate(lon0, center.Y);

                    var lon1 = center.X + r;
                    if (lon1 < -180)
                    {
                        lon1 += 360;
                    }

                    if (lon1 > 180)
                    {
                        lon1 -= 360;
                    }

                    var end = new Coordinate(lon1, center.Y);

                    var count = _random.Next(2, 5 + 1);
                    var dd = d / (count + 1);
                    var last = begin;
                    list.Add(begin);
                    for (int i = 0; i < count; i++)
                    {
                        var direction = _random.Next(0, 120 + 1) - 60;
                        var a = direction * ScienceMath.DegreesToRadians;
                        var yd = dd * Math.Tan(a);
                        var lon = last.X + dd;
                        if (lon < -180)
                        {
                            lon += 360;
                        }

                        if (lon > 180)
                        {
                            lon -= 360;
                        }

                        var point = new Coordinate(lon, last.Y + yd);

                        list.Add(point);

                        last = point;
                    }
                    list.Add(end);

                    return new GroundTarget()
                    {
                        Name = name,
                        Type = GroundTargetType.Route,
                        Points = new LineString(list.ToArray()),//Rotate(list, center, _random.Next(0, 360 + 1)),
                    };
                }
            case GroundTargetType.Area:
                {
                    var list = new List<Coordinate>();

                    var angle0 = (double)_random.Next(0, 90 + 1);
                    var vertexCount = _random.Next(4, 10 + 1);
                    var angleDelta = 360.0 / vertexCount;

                    for (int i = 0; i < vertexCount; i++)
                    {
                        var r = _random.Next(2, 10) / 10.0;
                        var a = angle0 * ScienceMath.DegreesToRadians;

                        var (dlon, dlat) = (r * Math.Cos(a), r * Math.Sin(a));

                        var lon = center.X + dlon;
                        if (lon < -180)
                        {
                            lon += 360;
                        }

                        if (lon > 180)
                        {
                            lon -= 360;
                        }

                        var point = new Coordinate(lon, center.Y + dlat);

                        list.Add(point);

                        angle0 -= angleDelta;
                    }

                    return new GroundTarget()
                    {
                        Name = name,
                        Type = GroundTargetType.Area,
                        Points = new LineString(list.ToArray()),
                    };
                }
            default:
                throw new Exception();
        }
    }
}
