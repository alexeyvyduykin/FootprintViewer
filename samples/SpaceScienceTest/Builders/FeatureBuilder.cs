using Mapsui;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using SpaceScienceTest.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceTest.Builders;

public static class FeatureBuilder
{
    public static IFeature CreateArrow(IEnumerable<(double lonDeg, double latDeg)> vertices)
    {
        var dd = vertices.TakeLast(2).ToList();
        var (x1, y1) = dd[0];
        var (x2, y2) = dd[1];
        var arrow = CreateArrowShape(x1, y1, x2, y2, 2.0);

        return arrow.ToFeature("Arrow");
    }

    private static Polygon CreateArrowShape(double x1, double y1, double x2, double y2, double len = 1.0)
    {
        // Backward direction vector
        var (dx, dy) = (x1 - x2, y1 - y2);

        // Length of it:
        var norm = Math.Sqrt(dx * dx + dy * dy);

        // Normalize it: uD =

        var (udx, udy) = (dx / norm, dy / norm);

        // To form "wings" of arrow, rotate uD by needed angle. For example, I use angle Pi / 6 with Cos(Pi/ 6) = Sqrt(3) / 2 and Sin(Pi/ 6) = 1 / 2

        var angle = Math.PI / 3;// Math.PI / 6;
        var cosa = Math.Cos(angle);
        var sina = Math.Sin(angle);

        var ax = udx * cosa - udy * sina;
        var ay = udx * sina + udy * cosa;
        var bx = udx * cosa + udy * sina;
        var by = -udx * sina + udy * cosa;


        // Points for head with wing length L = 20:
        var (a, b) = (x1 + len * ax, y1 + len * ay);
        var (c, d) = (x1 + len * bx, y1 + len * by);

        var list = new[]
        {
            (x2, y2),
            (a, b),
            (c, d),
            (x2, y2),
        };

        var coordinates = list
            .Select(s => SphericalMercator.FromLonLat(s.Item1, s.Item2))
            .Select(s => new Coordinate(s.x, s.y))
            .ToArray();

        var gf = new GeometryFactory();
        var poly = gf.CreatePolygon(coordinates);

        return poly!;
    }
}
