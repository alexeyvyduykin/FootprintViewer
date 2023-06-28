using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer;

public static class NetTopologySuiteExtensions
{
    public static Polygon ToLinearPolygon(this LineString lineString)
    {
        var points = lineString.MainCoordinates().SkipLast(1);
        var reversePoints = lineString.MainCoordinates();

        reversePoints.Reverse();
        reversePoints = reversePoints.SkipLast(1).ToList();

        var linearRing = points.Concat(reversePoints).ToLinearRing();
        return new Polygon(linearRing);
    }

    public static Coordinate[] ToClosedCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToList();

        var first = coordinates.First();

        if (first != coordinates.Last())
        {
            coordinates.Add(first);
        }

        return coordinates.ToArray();
    }

    public static Coordinate[] ToClosedCoordinates(this Coordinate[] coordinates)
    {
        var first = coordinates[0];

        var list = coordinates.ToList();

        if (first != list.Last())
        {
            list.Add(first);
        }

        return list.ToArray();
    }

    public static Coordinate[] ToGreaterThanTwoCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToArray();

        if (coordinates.Length >= 2)
        {
            return coordinates;
        }

        if (coordinates.Length == 0)
        {
            return new Coordinate[] { new Coordinate(0.0, 0.0), new Coordinate(0.0, 0.0) };
        }

        return new Coordinate[] { coordinates[0], coordinates[0] };
    }

    public static Coordinate[] ToCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToArray();

        return coordinates;
    }

    public static GeometryFeature ToFeatureEx(this Geometry geometry, string? name = null, IStyle? style = null)
    {
        var feature = geometry.ToFeature();

        if (string.IsNullOrEmpty(name) == false)
        {
            feature["Name"] = name;
        }

        if (style is { })
        {
            feature.Styles.Add(style);
        }

        return feature;
    }
}