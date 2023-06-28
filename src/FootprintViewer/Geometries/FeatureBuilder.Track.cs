using Mapsui;
using Mapsui.Projections;
using Mapsui.Styles;
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
            s => s.Value.Select(s => CreateLineString(name, s)).ToList());
    }

    public static Dictionary<int, List<IFeature>> CreateTracksVertices(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> tracks)
    {
        return tracks.ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => ToPointsFeatures(s)).ToList());
    }

    public static IFeature CreateTrack(string name, List<(double lonDeg, double latDeg)> list)
    {
        return CreateLineString(name, list);
    }

    public static IFeature CreateTrack(List<List<(double lonDeg, double latDeg)>> list, string? name = null, IStyle? style = null)
    {
        var lineStrings = new List<LineString>();

        foreach (var item in list)
        {
            var vertices = item.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

            var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

            lineStrings.Add(line);
        }

        return new MultiLineString(lineStrings.ToArray()).ToFeatureEx(name, style);
    }
}
