using FootprintViewer.Data;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using SpaceScience;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer;

public static class FeatureBuilder
{
    public static List<IFeature> Build(IEnumerable<GroundTarget> groundTargets)
    {
        return groundTargets.Select(s => Build(s)).ToList();
    }

    public static IFeature Build(GroundTarget groundTarget)
    {
        var geometry = groundTarget.Type switch
        {
            GroundTargetType.Point => new Point(SphericalMercator.FromLonLat(((Point)groundTarget.Points!).X, ((Point)groundTarget.Points!).Y).ToCoordinate()),
            GroundTargetType.Route => RouteCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
            GroundTargetType.Area => AreaCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
            _ => throw new Exception()
        };

        var feature = geometry.ToFeature();

        feature["Name"] = groundTarget.Name;
        feature["State"] = "Unselected";
        feature["Highlight"] = false;
        feature["Type"] = groundTarget.Type.ToString();

        return feature;
    }

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

    public static List<IFeature> Build(IEnumerable<Footprint> footprints)
    {
        return footprints.Select(s => Build(s)).ToList();
    }

    public static IFeature Build(Footprint footprint)
    {
        var poly = AreaCutting(footprint.Points!.Coordinates);

        var feature = poly.ToFeature(footprint.Name!);

        return feature;
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

    public static List<IFeature> Build(GroundStation groundStation)
    {
        var list = new List<IFeature>();

        var gs = GroundStationBuilder.Create(groundStation);

        var areaCount = gs.Areas.Count;

        bool isHole = (gs.InnerAngle != 0.0);

        // First area
        if (isHole == false)
        {
            var poligons = gs.Areas.First()
                .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat)))
                .Select(s => new GeometryFactory().CreatePolygon(s.ToClosedCoordinates()))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiPolygon(poligons);

            var feature = multi.ToFeature();

            feature["Count"] = $"{areaCount}";
            feature["Index"] = $"{0}";

            list.Add(feature);
        }

        // Areas
        if (areaCount > 1)
        {
            for (int i = 1; i < areaCount; i++)
            {
                var poligons = new List<Polygon>();

                var rings = gs.Areas[i - 1].Select(s => s.Reverse().Select(s => SphericalMercator.FromLonLat(s.lon, s.lat))).ToList();

                int index = 0;

                foreach (var points1 in gs.Areas[i])
                {
                    var res = points1.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                    if (index < rings.Count)
                    {
                        var interiorRings = (gs.Areas[i].Count() == 1) ? rings : new List<IEnumerable<(double, double)>>() { rings[index++] };

                        var shell = new GeometryFactory().CreateLinearRing(res.ToClosedCoordinates());
                        var holes = interiorRings.Select(s => new GeometryFactory().CreateLinearRing(s.ToClosedCoordinates())).ToArray();

                        var poly = new GeometryFactory().CreatePolygon(shell, holes);

                        poligons.Add(poly);
                    }
                    else
                    {
                        var poly = new GeometryFactory().CreatePolygon(res.ToClosedCoordinates());

                        poligons.Add(poly);
                    }
                }

                var multi = new GeometryFactory().CreateMultiPolygon(poligons.ToArray());

                var feature = multi.ToFeature();

                feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
                feature["Index"] = $"{(isHole == true ? i - 1 : i)}";

                list.Add(feature);
            }
        }

        // Inner border
        if (isHole == true)
        {
            var lineStrings = gs.InnerBorder
                .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var feature = multi.ToFeature();

            feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
            feature["InnerBorder"] = "";

            list.Add(feature);
        }

        // Outer border
        if (gs.OuterBorder.Any())
        {
            var lineStrings = gs.OuterBorder
                .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var feature = multi.ToFeature();

            feature["Count"] = $"{(isHole == true ? areaCount - 1 : areaCount)}";
            feature["OuterBorder"] = "";

            list.Add(feature);
        }

        return list;
    }

    public static Dictionary<int, List<IFeature>> Build(string name, Dictionary<int, List<List<(double lon, double lat)>>> swaths)
    {
        var dict = new Dictionary<int, List<IFeature>>();

        foreach (var item in swaths)
        {
            var list = item.Value.Select(s =>
            {
                var vertices = s.Select(s => SphericalMercator.FromLonLat(s.lon * ScienceMath.RadiansToDegrees, s.lat * ScienceMath.RadiansToDegrees));

                var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

                return (IFeature)poly.ToFeature(name);
            }).ToList();

            dict.Add(item.Key, list);
        }

        return dict;
    }

    public static Dictionary<int, List<IFeature>> BuildTrack(string name, Dictionary<int, List<List<(double lon, double lat)>>> tracks)
    {
        var dict = new Dictionary<int, List<IFeature>>();

        foreach (var item in tracks)
        {
            var list = item.Value.Select(s =>
            {
                var vertices = s.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

                return (IFeature)line.ToFeature(name);
            }).ToList();

            dict.Add(item.Key, list);
        }

        return dict;
    }
}
