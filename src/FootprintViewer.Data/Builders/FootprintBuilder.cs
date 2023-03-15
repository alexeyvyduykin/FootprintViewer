using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SpaceScience;
using SpaceScience.Model;

namespace FootprintViewer.Data.Builders;

internal static class FootprintBuilder
{
    private static readonly Random _random = new();
    private static readonly double _size = 1.2;
    private static readonly double _r = Math.Sqrt(_size * _size / 2.0);
    private static readonly int _durationMin = 10;
    private static readonly int _durationMax = 30;

    public static IList<Footprint> Create(IList<Satellite> satellites, int footprintCount)
    {
        var footprints = new List<Footprint>();
        int footprintIndex = 0;

        var satCount = satellites.Count;

        var footprintCountPerSat = (double)footprintCount / satCount;

        foreach (var satellite in satellites)
        {
            var sat = satellite.ToPRDCTSatellite();
            var sensor1 = satellite.ToPRDCTSensor(SwathDirection.Left);
            var sensor2 = satellite.ToPRDCTSensor(SwathDirection.Right);

            var swath1 = new Swath(sat.Orbit, sensor1.VerticalHalfAngleDEG, sensor1.RollAngleDEG);
            var swath2 = new Swath(sat.Orbit, sensor2.VerticalHalfAngleDEG, sensor2.RollAngleDEG);

            Swath[] bands = new Swath[] { swath1, swath2 };

            var epoch = sat.Orbit.Epoch;

            var nodes = sat.Nodes();

            var countPerNode = (int)(footprintCountPerSat / nodes.Count);

            var uDelta = 360.0 / countPerNode;

            for (int i = 0; i < nodes.Count; i++)
            {
                double uLast = 0.0;
                for (int j = 0; j < countPerNode; j++)
                {
                    double u1 = uLast;
                    double u2 = uLast + uDelta;

                    double u = u1 + (u2 - u1) / 2.0;
                    double duration = _random.Next(_durationMin, _durationMax + 1);

                    SwathDirection sensorIndex;

                    if (u >= 75 && u <= 105)
                    {
                        sensorIndex = SwathDirection.Left;
                    }
                    else if (u >= 255 && u <= 285)
                    {
                        sensorIndex = SwathDirection.Right;
                    }
                    else
                    {
                        sensorIndex = (SwathDirection)_random.Next(0, 1 + 1);
                    }

                    var (t, center, border) = GetRandomFootprint(sat, bands[(int)sensorIndex], nodes[i].Value - 1, u);

                    footprints.Add(new Footprint()
                    {
                        Name = $"Footprint{++footprintIndex:0000}",
                        TargetName = $"GroundTarget{footprintIndex:0000}",
                        SatelliteName = satellite.Name,
                        Center = center,
                        Points = new LineString(border.Select(s => new Coordinate(s.Lon, s.Lat)).ToArray()),
                        Begin = epoch.AddSeconds(t - duration / 2.0),
                        Duration = duration,
                        Node = nodes[i].Value,
                        Direction = sensorIndex,
                    });

                    uLast += uDelta;
                }

            }
        }

        return footprints;
    }

    private static double GetRandomAngle(double a1, double a2)
    {
        double d = Math.Floor(Math.Abs(a2 - a1));

        int dd = (int)Math.Floor(d / 2.0);

        var res = _random.Next(0, dd + 1) - dd / 2.0;

        var aCenter = Math.Min(a1, a2) + Math.Abs(a2 - a1) / 2.0;

        return aCenter + res / 2.0;
    }

    private static (double, Point, IEnumerable<Geo2D>) GetRandomFootprint(PRDCTSatellite satellite, Swath swath, int node, double u)
    {
        var list = new List<Geo2D>();

        var (t, c) = GetRandomCenterPoint(satellite, swath, node, u);
        var center = c.ToDegrees();

        double a = _random.Next(0, 90 + 1) * SpaceMath.DegreesToRadians;

        var (dlon1, dlat1) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon1 = center.Lon + dlon1;
        if (lon1 < -180 || lon1 > 180)
        {
            if (lon1 < -180)
            {
                lon1 += 360;
            }

            if (lon1 > 180)
            {
                lon1 -= 360;
            }
        }
        var res1 = new Geo2D(lon1, center.Lat + dlat1, GeoCoordTypes.Degrees);
        list.Add(res1);

        a -= Math.PI / 2.0;

        var (dlon2, dlat2) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon2 = center.Lon + dlon2;
        if (lon2 < -180 || lon2 > 180)
        {
            if (lon2 < -180)
            {
                lon2 += 360;
            }

            if (lon2 > 180)
            {
                lon2 -= 360;
            }
        }
        var res2 = new Geo2D(lon2, center.Lat + dlat2, GeoCoordTypes.Degrees);
        list.Add(res2);

        a -= Math.PI / 2.0;

        var (dlon3, dlat3) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon3 = center.Lon + dlon3;
        if (lon3 < -180 || lon3 > 180)
        {
            if (lon3 < -180)
            {
                lon3 += 360;
            }

            if (lon3 > 180)
            {
                lon3 -= 360;
            }
        }
        var res3 = new Geo2D(lon3, center.Lat + dlat3, GeoCoordTypes.Degrees);
        list.Add(res3);

        a -= Math.PI / 2.0;

        var (dlon4, dlat4) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon4 = center.Lon + dlon4;
        if (lon4 < -180 || lon4 > 180)
        {
            if (lon4 < -180)
            {
                lon4 += 360;
            }

            if (lon4 > 180)
            {
                lon4 -= 360;
            }
        }
        var res4 = new Geo2D(lon4, center.Lat + dlat4, GeoCoordTypes.Degrees);
        list.Add(res4);

        return (t, new Point(center.Lon, center.Lat), list);
    }

    private static (double, Geo2D) GetRandomCenterPoint(PRDCTSatellite satellite, Swath swath, int node, double u)
    {
        var a1 = swath.NearLine.Alpha1 * SpaceMath.RadiansToDegrees;
        var a2 = swath.FarLine.Alpha1 * SpaceMath.RadiansToDegrees;

        var angle = GetRandomAngle(a1, a2);

        var track = new CustomTrack(satellite.Orbit, angle, swath.NearLine.Direction);

        var (t, p) = GetGroundPoint(node, u, track, satellite);

        return (t, p);
    }

    private static (double, Geo2D) GetGroundPoint(int node, double u, CustomTrack track, PRDCTSatellite satellite)
    {
        var nodes = satellite.Nodes();

        double u0, u1, t0, t1;
        int quart;
        if (u >= 0.0 && u <= 90.0)
        {
            u0 = 0.0;
            u1 = 90.0;
            t0 = nodes[node].Quarts[0].TimeBegin;
            t1 = nodes[node].Quarts[0].TimeEnd;
            quart = nodes[node].Quarts[0].Quart;
        }
        else if (u >= 90.0 && u <= 180.0)
        {
            u0 = 90.0;
            u1 = 180.0;
            t0 = nodes[node].Quarts[1].TimeBegin;
            t1 = nodes[node].Quarts[1].TimeEnd;
            quart = nodes[node].Quarts[1].Quart;
        }
        else if (u >= 180.0 && u <= 270.0)
        {
            u0 = 180.0;
            u1 = 270.0;
            t0 = nodes[node].Quarts[2].TimeBegin;
            t1 = nodes[node].Quarts[2].TimeEnd;
            quart = nodes[node].Quarts[2].Quart;
        }
        else
        {
            u0 = 270.0;
            u1 = 360.0;
            t0 = nodes[node].Quarts[3].TimeBegin;
            t1 = nodes[node].Quarts[3].TimeEnd;
            quart = nodes[node].Quarts[3].Quart;
        }

        double t = (u - u0) * (t1 - t0) / (u1 - u0) + t0;

        var point = track.ContinuousTrack(node, t, satellite.TrueTimePastAN, quart);
        return (t, SpaceConverters.From180To180(point));
    }
}
