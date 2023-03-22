using SpaceScience;
using SpaceScience.Model;
using SpaceScienceSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Extensions;

public static class SpaceScienceExtensions
{
    public static List<(double lonDeg, double latDeg)> GetTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
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

    public static List<List<(double lonDeg, double latDeg)>> GetCutTrack(this GroundTrack track, int node, Func<double, double>? lonConverter = null)
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

    public static TrackBuilderResult BuildTracks(this PRDCTSatellite satellite)
    {
        var track = new GroundTrack(satellite.Orbit);

        track.CalculateTrackWithLogStep(100);

        var res = satellite
            .Nodes()
            .Select((_, index) => index)
            .ToDictionary(s => s, s => track.GetCutTrack(s, LonConverter));
        //  .ToDictionary(s => s, s => new List<List<(double,double)>> { track.GetTrack(s) });

        return new TrackBuilderResult(res);
    }

    public static SwathBuilderResult BuildSwaths(this PRDCTSatellite satellite, double lookAngleDeg, double radarAngleDeg)
    {
        var leftSwath = new Swath22(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathMode.Left);
        var rightSwath = new Swath22(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathMode.Right);

        var leftRes = BuildSwaths(satellite, leftSwath);
        var rightRes = BuildSwaths(satellite, rightSwath);

        return new SwathBuilderResult(leftRes, rightRes);
    }

    private static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildSwaths(PRDCTSatellite satellite, Swath22 swath)
    {
        var swaths = new Dictionary<int, List<List<(double lonDeg, double latDeg)>>>();

        var nodes = satellite.Nodes().Count;

        swath.CalculateSwathWithLogStep();

        for (int i = 0; i < nodes; i++)
        {
            var near = swath
                .GetNearTrack(i, LonConverter)
                .Select(s => new Geo2D(s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var far = swath
                .GetFarTrack(i, LonConverter)
                .Select(s => new Geo2D(s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => (g.Lon * SpaceMath.RadiansToDegrees, g.Lat * SpaceMath.RadiansToDegrees)).ToList()).ToList();

            swaths.Add(i, convertShapes);
        }

        return swaths;
    }

    private static double LonConverter(double lonDeg)
    {
        var value = lonDeg;
        while (value > 180) value -= 360.0;
        while (value < -180) value += 360.0;
        return value;
    }

    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath22 swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath22 swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, lonConverter);
    }
}
