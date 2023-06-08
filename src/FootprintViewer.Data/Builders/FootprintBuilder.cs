using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using ReactiveUI;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class FootprintBuilder
{
    private static readonly Random _random = new();
    private static readonly double _size = 1.2;
    private static readonly double _r = Math.Sqrt(_size * _size / 2.0);
    private static readonly int _durationMin = 10;
    private static readonly int _durationMax = 30;

    public static async Task<IList<Footprint>> CreateAsync(IList<Satellite> satellites, int count)
        => await Observable.Start(() => Create(satellites, count), RxApp.TaskpoolScheduler);

    public static IList<Footprint> Create(IList<Satellite> satellites, int footprintCount)
    {
        var footprints = new List<Footprint>();
        int footprintIndex = 0;

        var satCount = satellites.Count;

        var footprintCountPerSat = (double)footprintCount / satCount;

        foreach (var satellite in satellites)
        {
            var orbit = satellite.ToOrbit();

            var swath1 = new Swath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, SpaceScience.Model.SwathDirection.Left);
            var swath2 = new Swath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, SpaceScience.Model.SwathDirection.Right);

            var bands = new[] { swath1, swath2 };

            var epoch = orbit.Epoch;

            var nodes = orbit.NodesOnDay();

            var countPerNode = (int)(footprintCountPerSat / nodes);

            var uDeltaDeg = 360.0 / countPerNode;

            for (int node = 1; node <= nodes; node++)
            {
                double uLastDeg = 0.0;
                for (int j = 0; j < countPerNode; j++)
                {
                    double u1Deg = uLastDeg;
                    double u2Deg = uLastDeg + uDeltaDeg;

                    double uDeg = u1Deg + (u2Deg - u1Deg) / 2.0;
                    double duration = _random.Next(_durationMin, _durationMax + 1);

                    Models.SwathDirection sensorIndex;

                    if (uDeg >= 75 && uDeg <= 105)
                    {
                        sensorIndex = Models.SwathDirection.Left;
                    }
                    else if (uDeg >= 255 && uDeg <= 285)
                    {
                        sensorIndex = Models.SwathDirection.Right;
                    }
                    else
                    {
                        sensorIndex = (Models.SwathDirection)_random.Next(0, 1 + 1);
                    }

                    var (t, center, border) = GetRandomFootprint(orbit, bands[(int)sensorIndex], node, uDeg);

                    footprints.Add(new Footprint()
                    {
                        Name = $"Footprint{++footprintIndex:0000}",
                        TargetName = $"GroundTarget{footprintIndex:0000}",
                        SatelliteName = satellite.Name,
                        Center = center,
                        Border = new LineString(border),
                        Begin = epoch.AddSeconds(t - duration / 2.0),
                        Duration = duration,
                        Node = node,
                        Direction = sensorIndex,
                    });

                    uLastDeg += uDeltaDeg;
                }

            }
        }

        return footprints;
    }

    public static Footprint CreateRandom()
    {
        return new Footprint()
        {
            Name = $"Footprint{_random.Next(1, 101):000}",
            SatelliteName = $"Satellite{_random.Next(1, 10):00}",
            Center = new Point(_random.Next(-180, 180), _random.Next(-90, 90)),
            Begin = DateTime.Now,
            Duration = _random.Next(20, 40),
            Node = _random.Next(1, 16),
            Direction = (Models.SwathDirection)_random.Next(0, 2),
        };
    }

    private static double GetRandomAngle(double a1, double a2)
    {
        double d = Math.Floor(Math.Abs(a2 - a1));

        int dd = (int)Math.Floor(d / 2.0);

        var res = _random.Next(0, dd + 1) - dd / 2.0;

        var aCenter = Math.Min(a1, a2) + Math.Abs(a2 - a1) / 2.0;

        return aCenter + res / 2.0;
    }

    public static LineString CreateFootprintBorder(double lonDeg, double latDeg)
    {
        double a = _random.Next(0, 90 + 1) * SpaceMath.DegreesToRadians;

        var (dlon1, dlat1) = (_r * Math.Cos(a), _r * Math.Sin(a));

        a -= Math.PI / 2.0;

        var (dlon2, dlat2) = (_r * Math.Cos(a), _r * Math.Sin(a));

        a -= Math.PI / 2.0;

        var (dlon3, dlat3) = (_r * Math.Cos(a), _r * Math.Sin(a));

        a -= Math.PI / 2.0;

        var (dlon4, dlat4) = (_r * Math.Cos(a), _r * Math.Sin(a));

        var border = new[]
        {
            (LonConverters.Default(lonDeg + dlon1), latDeg + dlat1),
            (LonConverters.Default(lonDeg + dlon2), latDeg + dlat2),
            (LonConverters.Default(lonDeg + dlon3), latDeg + dlat3),
            (LonConverters.Default(lonDeg + dlon4), latDeg + dlat4),
            (LonConverters.Default(lonDeg + dlon1), latDeg + dlat1)
        };

        var coords = border.Select(s => new Coordinate(s.Item1, s.Item2)).ToArray();

        return new LineString(coords);
    }

    private static (double, (double lonDeg, double latDeg)) GetRandomCenterPoint(Orbit orbit, Swath swath, int node, double uDeg)
    {
        var a1 = swath.NearTrack.AngleDeg;
        var a2 = swath.FarTrack.AngleDeg;

        var angleDeg = GetRandomAngle(a1, a2);

        var dir = swath.Direction;

        var trackDir = dir switch
        {
            SpaceScience.Model.SwathDirection.Left => TrackDirection.Left,
            SpaceScience.Model.SwathDirection.Right => TrackDirection.Right,
            SpaceScience.Model.SwathDirection.Middle => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };

        var factor = new FactorShiftTrack(orbit, a1, a2, dir);
        var track = new GroundTrack(orbit, factor, angleDeg, trackDir);

        var uRad = uDeg * SpaceMath.DegreesToRadians;

        track.CalculateTrack(uRad, uRad, 1);

        var (lonDeg, latDeg, _, t) = track.GetFullTrack(node, LonConverters.Default).FirstOrDefault();

        return (t, (lonDeg, latDeg));
    }

    public static (double, Point, Coordinate[]) GetRandomFootprint(Orbit orbit, Swath swath, int node, double uDeg)
    {
        var list = new List<(double lonDeg, double latDeg)>();

        var (t, (centerLonDeg, centerLatDeg)) = GetRandomCenterPoint(orbit, swath, node, uDeg);

        double a = _random.Next(0, 90 + 1) * SpaceMath.DegreesToRadians;

        var (dlon1, dlat1) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon1 = centerLonDeg + dlon1;
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
        var res1 = (lon1, centerLatDeg + dlat1);
        list.Add(res1);

        a -= Math.PI / 2.0;

        var (dlon2, dlat2) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon2 = centerLonDeg + dlon2;
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
        var res2 = (lon2, centerLatDeg + dlat2);
        list.Add(res2);

        a -= Math.PI / 2.0;

        var (dlon3, dlat3) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon3 = centerLonDeg + dlon3;
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
        var res3 = (lon3, centerLatDeg + dlat3);
        list.Add(res3);

        a -= Math.PI / 2.0;

        var (dlon4, dlat4) = (_r * Math.Cos(a), _r * Math.Sin(a));
        var lon4 = centerLonDeg + dlon4;
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
        var res4 = (lon4, centerLatDeg + dlat4);
        list.Add(res4);

        return (t, new Point(centerLonDeg, centerLatDeg), list.Select(s => new Coordinate(s.lonDeg, s.latDeg)).ToArray());
    }
}