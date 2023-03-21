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
        var leftSwath = new Swath(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathMode.Left);
        var rightSwath = new Swath(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathMode.Right);

        var leftRes = BuildSwaths(satellite, leftSwath);
        var rightRes = BuildSwaths(satellite, rightSwath);

        return new SwathBuilderResult(leftRes, rightRes);
    }

    private static Dictionary<int, List<List<(double lon, double lat)>>> BuildSwaths(PRDCTSatellite satellite, Swath swath)
    {
        var swaths = new Dictionary<int, List<List<(double, double)>>>();

        foreach (var node in satellite.Nodes().Select(s => s.Value))
        {
            var near = swath.GetNearGroundTrack(satellite, node - 1, SpaceConverters.From180To180);
            var far = swath.GetFarGroundTrack(satellite, node - 1, SpaceConverters.From180To180);

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => g.Deconstruct()).ToList()).ToList();

            swaths.Add(node, convertShapes);
        }

        return swaths;
    }
}