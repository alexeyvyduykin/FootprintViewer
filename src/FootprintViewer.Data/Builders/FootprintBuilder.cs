using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;

namespace FootprintViewer.Data.Builders;

public static class FootprintBuilder
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

            var swath1 = new Swath(sat.Orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, SpaceScience.Model.SwathDirection.Left);
            var swath2 = new Swath(sat.Orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, SpaceScience.Model.SwathDirection.Right);

            var bands = new[] { swath1, swath2 };

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

                    Models.SwathDirection sensorIndex;

                    if (u >= 75 && u <= 105)
                    {
                        sensorIndex = Models.SwathDirection.Left;
                    }
                    else if (u >= 255 && u <= 285)
                    {
                        sensorIndex = Models.SwathDirection.Right;
                    }
                    else
                    {
                        sensorIndex = (Models.SwathDirection)_random.Next(0, 1 + 1);
                    }

                    var (t, center, border) = GetRandomFootprint(sat.Orbit, bands[(int)sensorIndex], nodes[i].Value - 1, u);

                    footprints.Add(new Footprint()
                    {
                        Name = $"Footprint{++footprintIndex:0000}",
                        TargetName = $"GroundTarget{footprintIndex:0000}",
                        SatelliteName = satellite.Name,
                        Center = center,
                        Points = new LineString(border.Select(s => new Coordinate(s.lonRad, s.latRad)).ToArray()),
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

    private static (double, Point, IEnumerable<(double lonRad, double latRad)>) GetRandomFootprint(Orbit orbit, Swath swath, int node, double u)
    {
        var list = new List<(double lonRad, double latRad)>();

        var (t, c) = GetRandomCenterPoint(orbit, swath, node, u);
        var centerLonDeg = c.lonRad * SpaceMath.RadiansToDegrees;
        var centerLatDeg = c.latRad * SpaceMath.RadiansToDegrees;

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

        return (t, new Point(centerLonDeg, centerLatDeg), list);
    }

    private static (double, (double lonRad, double latRad)) GetRandomCenterPoint(Orbit orbit, Swath swath, int node, double u)
    {
        var a1 = swath.NearTrack.AngleDeg;
        var a2 = swath.FarTrack.AngleDeg;

        var angle = GetRandomAngle(a1, a2);

        var dir = swath.NearTrack.Direction;

        SpaceScience.Model.SwathDirection mode = dir switch
        {
            TrackDirection.None => SpaceScience.Model.SwathDirection.Middle,
            TrackDirection.Left => SpaceScience.Model.SwathDirection.Left,
            TrackDirection.Right => SpaceScience.Model.SwathDirection.Right,
            _ => SpaceScience.Model.SwathDirection.Middle
        };

        var factor = new FactorShiftTrack(orbit, a1, a2, mode);
        var track = new GroundTrack(orbit, factor, angle, dir);

        var (t, p) = GetGroundPoint(node, u, track);

        return (t, p);
    }

    private static (double, (double lonRad, double latRad)) GetGroundPoint(int node, double u, GroundTrack track)
    {
        var counts = 10;

        if (u >= 0.0 && u <= 90.0)
        {
            track.CalculateTrack(0.0, Math.Tau / 4.0, counts);
        }
        else if (u >= 90.0 && u <= 180.0)
        {
            track.CalculateTrack(Math.Tau / 4.0, Math.Tau / 2.0, counts);
        }
        else if (u >= 180.0 && u <= 270.0)
        {
            track.CalculateTrack(Math.Tau / 2.0, 3.0 * Math.Tau / 4.0, counts);
        }
        else
        {
            track.CalculateTrack(3.0 * Math.Tau / 4.0, Math.Tau, counts);
        }

        Random random = new();
        var i = random.Next(0, counts);
        var (lonDeg, latDeg, _, t) = track.GetFullTrackOfIndex(i, node, LonConverters.Default);

        return (t, (lonDeg * SpaceMath.DegreesToRadians, latDeg * SpaceMath.DegreesToRadians));
    }
}