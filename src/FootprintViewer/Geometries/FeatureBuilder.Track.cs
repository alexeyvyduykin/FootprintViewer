using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Geometries;

public static partial class FeatureBuilder
{
    public static Dictionary<int, List<IFeature>> CreateTracks(string name, Dictionary<int, List<List<(double lonDeg, double latDeg)>>> tracks)
    {
        return tracks.ToDictionary(
            s => s.Key,
            s => s.Value.Select(s => ToLineStringFeature(name, s)).ToList());
    }

    public static IFeature ToLineStringFeature(string name, List<(double lonDeg, double latDeg)> list)
    {
        var vertices = list.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

        var feature = line.ToFeature();

        feature["Name"] = name;

        return (IFeature)feature;
    }

    public static Dictionary<int, List<IFeature>> CreateTracksVertices(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> tracks)
    {
        return tracks.ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => ToPointsFeatures(s)).ToList());
    }
}
