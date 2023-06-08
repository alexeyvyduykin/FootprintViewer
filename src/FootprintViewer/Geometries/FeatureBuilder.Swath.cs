using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Geometries;

public static partial class FeatureBuilder
{
    public static Dictionary<int, List<IFeature>> CreateSwaths(string name, Dictionary<int, List<List<(double lonDeg, double latDeg)>>> swaths)
    {
        return swaths.ToDictionary(
            s => s.Key,
            s => s.Value.Select(s => ToPolygonFeature(name, s)).ToList());
    }

    private static IFeature ToPolygonFeature(string name, List<(double lonDeg, double latDeg)> list)
    {
        var vertices = list.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

        var feature = poly.ToFeature();

        feature["Name"] = name;

        return (IFeature)feature;
    }

    public static Dictionary<int, List<IFeature>> CreateSwathsVertices(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> swaths)
    {
        return swaths.ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => ToPointsFeatures(s)).ToList());
    }

    private static List<IFeature> ToPointsFeatures(List<(double lonDeg, double latDeg)> list)
    {
        return list
            .Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg))
            .ToCoordinates()
            .Select(s => new GeometryFactory().CreatePoint(s))
            .Select(s => (IFeature)s.ToFeature())
            .ToList();
    }
}
