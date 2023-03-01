using NetTopologySuite.Geometries;

namespace FootprintViewer.Data.Extensions;

public static class NetTopologySuiteExtensions
{
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
}
