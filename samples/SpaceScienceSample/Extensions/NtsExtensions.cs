using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Extensions;

public static class NtsExtensions
{
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
}
