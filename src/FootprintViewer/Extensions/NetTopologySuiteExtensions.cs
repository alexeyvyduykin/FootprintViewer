using Mapsui.Interactivity;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System;
using System.Linq;

namespace FootprintViewer;

public static class NetTopologySuiteExtensions
{
    public static Polygon ToLinearPolygon(this LineString lineString)
    {
        var points = lineString.MainCoordinates().SkipLast(1);
        var reversePoints = lineString.MainCoordinates().Reverse().SkipLast(1);
        var linearRing = points.Concat(reversePoints).ToLinearRing();
        return new Polygon(linearRing);
    }
}
