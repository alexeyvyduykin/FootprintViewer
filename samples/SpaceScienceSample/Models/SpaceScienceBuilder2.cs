using SpaceScience;
using SpaceScience.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Models;
public class TrackBuilderResult22
{
    private readonly Dictionary<int, List<List<(double, double)>>> _tracks;

    public TrackBuilderResult22(Dictionary<int, List<List<(double, double)>>> tracks)
    {
        _tracks = tracks;
    }

    public Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Track => _tracks;
}

public static class SpaceScienceBuilder2
{
    private static double LonConverter(double lonDeg)
    {
        var value = lonDeg;
        while (value > 180) value -= 360.0;
        while (value < -180) value += 360.0;
        return value;
    }

    public static SwathBuilderResult BuildSwaths(PRDCTSatellite satellite, double lookAngleDeg, double radarAngleDeg)
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

        for (int i = 0; i < nodes; i++)
        {
            var near = swath.GetNearGroundTrack(satellite, i/*, SpaceConverters.From180To180*/);
            var far = swath.GetFarGroundTrack(satellite, i);//, SpaceConverters.From180To180);

            var engine2D = new SwathCore22(
                near.Select(s => new Geo2D(s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians)).ToList(), 
                far.Select(s => new Geo2D(s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians)).ToList(), 
                swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => (g.Lon * SpaceMath.RadiansToDegrees, g.Lat * SpaceMath.RadiansToDegrees)).ToList()).ToList();

            swaths.Add(i, convertShapes);
        }

        return swaths;
    }
}