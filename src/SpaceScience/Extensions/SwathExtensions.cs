﻿using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class SwathExtensions
{
    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath swath, int node, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetNearTrack(this Swath swath, int node, double duration, Func<double, double>? lonConverter = null)
    {
        return swath.NearTrack.GetTrack(node, duration, lonConverter);
    }

    public static List<(double lonDeg, double latDeg)> GetFarTrack(this Swath swath, int node, double duration, Func<double, double>? lonConverter = null)
    {
        return swath.FarTrack.GetTrack(node, duration, lonConverter);
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildSwaths(this Swath swath, Orbit orbit)
    {
        var swaths = new Dictionary<int, List<List<(double lonDeg, double latDeg)>>>();

        swath.CalculateSwathWithLogStep();

        var nodes = orbit.NodesOnDay();

        for (int i = 0; i < nodes; i++)
        {
            var near = swath
                .GetNearTrack(i, LonConverters.Default)
                .Select(s => (s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var far = swath
                .GetFarTrack(i, LonConverters.Default)
                .Select(s => (s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => (g.lonRad * SpaceMath.RadiansToDegrees, g.latRad * SpaceMath.RadiansToDegrees)).ToList()).ToList();

            swaths.Add(i, convertShapes);
        }

        return swaths;
    }
}
