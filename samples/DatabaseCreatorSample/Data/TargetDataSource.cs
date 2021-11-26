using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseCreatorSample.Data
{
    public class TargetDataSource
    {
        private static Random _random = new Random();
        private static int _count = 5000;
        private readonly List<GroundTarget> _targets = new List<GroundTarget>();

        public TargetDataSource(IList<Footprint> footprints)
        {
            int counts = footprints.Count;

            int index = 0;

            for (int i = 0; i < counts; i++)
            {
                var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

                var target = CreateRandomTarget($"GroundTarget{++index:0000}", type, footprints[i].Center);

                footprints[i].TargetName = target.Name;

                _targets.Add(target);
            }

            for (int i = 0; i < _count - counts; i++)
            {
                var type = (GroundTargetType)Enum.ToObject(typeof(GroundTargetType), _random.Next(0, 2 + 1));

                var center = new Point(_random.Next(-180, 180 + 1), _random.Next(-80, 80 + 1));

                var target = CreateRandomTarget($"GroundTarget{++index:0000}", type, center);

                _targets.Add(target);
            }

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
                        Points = new[] { center },
                    };
                case GroundTargetType.Route:
                {
                    var list = new List<Point>();
                    double r = _random.Next(10, 20 + 1) / 10.0;
                    double d = 2 * r;
                    
                    var lon0 = center.X - r;
                    if (lon0 < -180) lon0 += 360;
                    if (lon0 > 180) lon0 -= 360;
                    var begin = new Point(lon0, center.Y);
                  
                    var lon1 = center.X + r;
                    if (lon1 < -180) lon1 += 360;
                    if (lon1 > 180) lon1 -= 360;
                    var end = new Point(lon1, center.Y);

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
                        if (lon < -180) lon += 360;
                        if (lon > 180) lon -= 360;
                        var point = new Point(lon, last.Y + yd);

                        list.Add(point);

                        last = point;
                    }
                    list.Add(end);

                    return new GroundTarget()
                    {
                        Name = name,
                        Type = GroundTargetType.Route,
                        Points = list,//Rotate(list, center, _random.Next(0, 360 + 1)),
                    };
                }
                case GroundTargetType.Area:
                {
                    var list = new List<Point>();

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
                            lon += 360;
                        if (lon > 180)
                            lon -= 360;

                        var point = new Point(lon, center.Y + dlat);

                        list.Add(point);

                        angle0 -= angleDelta;
                    }

                    return new GroundTarget()
                    {
                        Name = name,
                        Type = GroundTargetType.Area,
                        Points = list,
                    };
                }
                default:
                    throw new Exception();
            }
        }

        private static Point Rotate(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        private static IEnumerable<Point> Rotate(IEnumerable<Point> pointsToRotate, Point centerPoint, double angleInDegrees)
        {
            var list = new List<Point>();
            foreach (var item in pointsToRotate)
            {
                list.Add(Rotate(item, centerPoint, angleInDegrees));
            }
            return list;
        }

        public IEnumerable<GroundTarget> Targets => _targets;
    }
}
