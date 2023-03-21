using SpaceScience;
using SpaceScience.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Models;

public class FactorTrack22
{
    private readonly double _earthRotationSec = 86164.09053;
    private readonly FactorShiftTrack _factor;
    private readonly Orbit _orbit;
    private readonly double _angleRad;
    private readonly double _period;
    private readonly int _direction;
    private readonly List<(double lonDeg, double latDeg)> _cacheTrack = new();

    public FactorTrack22(Orbit orbit)
    {
        _angleRad = 0.0;// angleDeg * SpaceMath.DegreesToRadians;       
        _orbit = orbit;
        _factor = new FactorShiftTrack(_orbit, 0.0, 0.0, SwathMode.Middle);
        _direction = 0;
        _period = orbit.Period;
        //_direction = direction switch
        //{
        //    TrackPointDirection.None => 0,
        //    TrackPointDirection.Left => -1,
        //    TrackPointDirection.Right => 1,
        //    _ => 0,
        //};
    }

    public FactorTrack22(Orbit orbit, FactorShiftTrack factor, double angleDeg, SwathMode mode)
    {
        _angleRad = angleDeg * SpaceMath.DegreesToRadians;       
        _orbit = orbit;
        _factor = factor;      
        _period = orbit.Period;
        _direction = mode switch
        {
            SwathMode.Middle => 0,
            SwathMode.Left => -1,
            SwathMode.Right => 1,
            _ => 0,
        };
    }

    public List<(double lonDeg, double latDeg)> CacheTrack => _cacheTrack;

    public double NodeOffsetDeg => 360.0 * _factor.Offset;

    public double EarthRotateOffsetDeg => (_period / _earthRotationSec) * 360.0 * _factor.Offset;

    public void CalculateTrackWithLogStep(int counts)
    {
        _cacheTrack.Clear();

        var slices = (int)Math.Ceiling(counts / 4.0);

        // [0 - 90)
        foreach (var item in LogStep(0.0, 90, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var (lon, lat) = ContinuousTrack22(u);

            _cacheTrack.Add((lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees));
        }

        // [90 - 180)
        foreach (var item in LogStepReverse(180, 90, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var (lon, lat) = ContinuousTrack22(u);

            _cacheTrack.Add((lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees));
        }

        // [180 - 270)
        foreach (var item in LogStep(180, 270, slices).SkipLast(1))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var (lon, lat) = ContinuousTrack22(u);

            _cacheTrack.Add((lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees));
        }

        // [270 - 360]
        foreach (var item in LogStepReverse(360, 270, slices))
        {
            var u = item * SpaceMath.DegreesToRadians;

            var (lon, lat) = ContinuousTrack22(u);

            _cacheTrack.Add((lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees));
        }
        return;
    }

    public void CalculateTrack(double dt = 60.0)
    {
        var period = _orbit.Period;

        _cacheTrack.Clear();

        for (double t = 0; t < period; t += dt)
        {
            var u = _orbit.Anomalia(t);

            var (lon, lat) = ContinuousTrack22(u);

            _cacheTrack.Add((lon * SpaceMath.RadiansToDegrees, lat * SpaceMath.RadiansToDegrees));
        }

        return;
    }

    private static IEnumerable<double> LogStep(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[slices] = lower_bound;

        return arr;
    }

    private static IEnumerable<double> LogStepReverse(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[slices - i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[0] = lower_bound;

        return arr;
    }

    private Geo2D ContinuousTrack22(double u)
    {
        double semi_axis = (_orbit.Eccentricity == 0.0) ? _orbit.SemimajorAxis : _orbit.Semiaxis(u);
        double angle = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(_angleRad) / Constants.Re) - _angleRad;
        double uTr = (angle == 0.0) ? u : Math.Acos(Math.Cos(angle) * Math.Cos(u));
        double iTr = _orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _direction;
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
        {
            asinlon = SpaceMath.Sign(asinlon);
        }

        double lon = Math.Asin(asinlon);

        if (u >= 0.0 && u < SpaceMath.HALFPI)
        {
            lon = 0 + lon;
        }
        else if (u >= SpaceMath.HALFPI && u < 3.0 * SpaceMath.HALFPI)
        {
            lon = Math.PI - lon - _factor.Quart23 * SpaceMath.TWOPI;
        }
        else if (u >= 3.0 * SpaceMath.HALFPI && u <= SpaceMath.TWOPI)
        {
            lon = SpaceMath.TWOPI + lon - _factor.Quart4 * SpaceMath.TWOPI;
        }

        lon = lon - (_period / _earthRotationSec) * u;

        //   lon = lon + SpaceMath.TWOPI * _factor.Offset;
        return new Geo2D(lon, lat, GeoCoordTypes.Radians);
    }
}

public static class FactorTrack22Extensions
{
    public static List<(double lonDeg, double latDeg)> GetTrack(this FactorTrack22 track, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

        if (lonConverter != null)
        {
            return track.CacheTrack
                .Select(s => (lonConverter.Invoke(s.lonDeg + offset), s.latDeg))
                .ToList();
        }

        return track.CacheTrack
            .Select(s => (s.lonDeg + offset, s.latDeg))
            .ToList();
    }

    public static List<List<(double lonDeg, double latDeg)>> GetCutTrack(this FactorTrack22 track, int node, Func<double, double>? lonConverter = null)
    {
        var res = new List<List<(double, double)>>();

        var list = track.GetTrack(node, lonConverter);

        var temp = new List<(double, double)>();

        var (prevLonDeg, prevLatDeg) = list.FirstOrDefault();

        foreach (var (curLonDeg, curLatDeg) in list)
        {
            if (Math.Abs((curLonDeg - prevLonDeg) * SpaceMath.DegreesToRadians) >= 3.2)
            {
                var cutLatDeg = LinearInterpDiscontLat(prevLonDeg, prevLatDeg, curLonDeg, curLatDeg);

                if (prevLonDeg > 0.0)
                {
                    temp.Add((180.0, cutLatDeg));
                    res.Add(temp);
                    temp = new() { (-180.0, cutLatDeg), (curLonDeg, curLatDeg) };
                }
                else
                {
                    temp.Add((-180.0, cutLatDeg));
                    res.Add(temp);
                    temp = new() { (180.0, cutLatDeg), (curLonDeg, curLatDeg) };
                }
            }
            else
            {
                temp.Add((curLonDeg, curLatDeg));
            }

            prevLonDeg = curLonDeg;
            prevLatDeg = curLatDeg;
        }

        res.Add(temp);

        return res;
    }

    private static double LinearInterpDiscontLat(double lonDeg1, double latDeg1, double lonDeg2, double latDeg2)
    {
        if (lonDeg1 > lonDeg2)
        {
            lonDeg2 += 360.0;
        }
        else
        {
            lonDeg1 += 360.0;
        }

        return (latDeg1 + (180.0 - lonDeg1) * (latDeg2 - latDeg1) / (lonDeg2 - lonDeg1));
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildTracks(this PRDCTSatellite satellite)
    {
        var track = new FactorTrack22(satellite.Orbit);

        track.CalculateTrackWithLogStep(100);

        var res = satellite
            .Nodes()
            .Select((_, index) => index)
            .ToDictionary(s => s, s => track.GetCutTrack(s, LonConverter));
        //  .ToDictionary(s => s, s => new List<List<(double,double)>> { track.GetTrack(s) });

        return res;
    }

    private static double LonConverter(double lonDeg)
    {
        var value = lonDeg;
        while (value > 180) value -= 360.0;
        while (value < -180) value += 360.0;
        return value;
    }
}