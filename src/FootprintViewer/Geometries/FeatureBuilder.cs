using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Geometries;

public static partial class FeatureBuilder
{
    private static Geometry AreaCutting(IList<Point> points)
    {
        var count = points.Count;

        var vertices1 = new List<(double, double)>();
        var vertices2 = new List<(double, double)>();
        var vertices = vertices1;

        for (int i = 0; i < count; i++)
        {
            var p1 = points[i];
            var p2 = (i == count - 1) ? points[0] : points[i + 1];

            var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
            vertices.Add(point1);

            if (Math.Abs(p2.X - p1.X) > 180)
            {
                if (p2.X - p1.X > 0) // -180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp2);
                }

                if (p2.X - p1.X < 0) // +180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp2);
                }
            }
        }

        if (vertices2.Count != 0) // multipolygon
        {
            var poly1 = new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
            var poly2 = new GeometryFactory().CreatePolygon(vertices2.ToClosedCoordinates());

            return new GeometryFactory().CreateMultiPolygon(new[] { poly1, poly2 });
        }
        else
        {
            return new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
        }
    }

    private static Geometry RouteCutting(IList<Point> points)
    {
        var count = points.Count;

        var vertices1 = new List<(double, double)>();
        var vertices2 = new List<(double, double)>();
        var vertices = vertices1;

        for (int i = 0; i < count - 1; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];

            var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
            vertices.Add(point1);

            if (Math.Abs(p2.X - p1.X) > 180)
            {
                if (p2.X - p1.X > 0) // -180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp2);
                }

                if (p2.X - p1.X < 0) // +180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp2);
                }
            }
        }

        if (vertices2.Count != 0) // MultiLineString
        {
            var line1 = new LineString(vertices1.ToGreaterThanTwoCoordinates());
            var line2 = new LineString(vertices2.ToGreaterThanTwoCoordinates());

            return new GeometryFactory().CreateMultiLineString(new[] { line1, line2 });
        }
        else
        {
            return new LineString(vertices1.ToGreaterThanTwoCoordinates());
        }
    }

    private static double LinearInterpDiscontLat(Point p1, Point p2)
    {
        // one longitude should be negative one positive, make them both positive
        double lon1 = p1.X, lat1 = p1.Y, lon2 = p2.X, lat2 = p2.Y;
        if (lon1 > lon2)
        {
            lon2 += 360;
        }
        else
        {
            lon1 += 360;
        }

        return (lat1 + (180 - lon1) * (lat2 - lat1) / (lon2 - lon1));
    }

    private static Geometry AreaCutting(Coordinate[] points)
    {
        var count = points.Length;
        var vertices1 = new List<(double, double)>();
        var vertices2 = new List<(double, double)>();
        var vertices = vertices1;

        for (int i = 0; i < count; i++)
        {
            var p1 = points[i];
            var p2 = (i == count - 1) ? points[0] : points[i + 1];

            var point1 = SphericalMercator.FromLonLat(p1.X, p1.Y);
            vertices.Add(point1);

            if (Math.Abs(p2.X - p1.X) > 180)
            {
                if (p2.X - p1.X > 0) // -180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp2);
                }

                if (p2.X - p1.X < 0) // +180 cutting
                {
                    var cutLat = LinearInterpDiscontLat(p1, p2);
                    var pp1 = SphericalMercator.FromLonLat(180, cutLat);
                    vertices.Add(pp1);

                    vertices = (vertices == vertices1) ? vertices2 : vertices1;

                    var pp2 = SphericalMercator.FromLonLat(-180, cutLat);
                    vertices.Add(pp2);
                }
            }
        }

        if (vertices2.Count != 0) // multipolygon
        {
            var poly1 = new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
            var poly2 = new GeometryFactory().CreatePolygon(vertices2.ToClosedCoordinates());

            return new GeometryFactory().CreateMultiPolygon(new[] { poly1, poly2 });
        }
        else
        {
            return new GeometryFactory().CreatePolygon(vertices1.ToClosedCoordinates());
        }
    }

    private static double LinearInterpDiscontLat(Coordinate p1, Coordinate p2)
    {
        // one longitude should be negative one positive, make them both positive
        double lon1 = p1.X, lat1 = p1.Y, lon2 = p2.X, lat2 = p2.Y;
        if (lon1 > lon2)
        {
            lon2 += 360;
        }
        else
        {
            lon1 += 360;
        }

        return (lat1 + (180 - lon1) * (lat2 - lat1) / (lon2 - lon1));
    }
}
