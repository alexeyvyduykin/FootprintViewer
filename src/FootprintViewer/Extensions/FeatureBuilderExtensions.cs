using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using SpaceScience.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static SpaceScience.Extensions.SatelliteExtensions;

namespace FootprintViewer.Extensions;

public static class FeatureBuilderExtensions
{
    public static Dictionary<int, List<IFeature>> ToFeature(this SwathBuilderResult result, string name, SwathDirection mode)
    {
        return mode switch
        {
            SwathDirection.Middle => new(),
            SwathDirection.Left => result.Left.ToDictionary(
                                s => s.Key,
                                s => s.Value.Select(s => s.ToPolygonFeature(name)).ToList()),
            SwathDirection.Right => result.Right.ToDictionary(
                                s => s.Key,
                                s => s.Value.Select(s => s.ToPolygonFeature(name)).ToList()),
            _ => throw new Exception(),
        };
    }

    public static IFeature ToPolygonFeature(this List<(double lonDeg, double latDeg)> list, string name)
    {
        var vertices = list.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

        var feature = poly.ToFeature();

        feature["Name"] = name;

        return (IFeature)feature;
    }

    public static Dictionary<int, List<IFeature>> ToFeature(this TrackBuilderResult result, string name)
    {
        return result.Track.ToDictionary(
            s => s.Key,
            s => s.Value.Select(s => s.ToLineStringFeature(name)).ToList());
    }

    public static IFeature ToLineStringFeature(this List<(double lonDeg, double latDeg)> list, string name)
    {
        var vertices = list.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

        var feature = line.ToFeature();

        feature["Name"] = name;

        return (IFeature)feature;
    }

    public static Dictionary<int, List<IFeature>> ToFeatureVertices(this SwathBuilderResult result, SwathDirection mode)
    {
        return mode switch
        {
            SwathDirection.Middle => new(),
            SwathDirection.Left => result.Left.ToDictionary(
                                s => s.Key,
                                s => s.Value.SelectMany(s => s.ToPointsFeatures()).ToList()),
            SwathDirection.Right => result.Right.ToDictionary(
                                s => s.Key,
                                s => s.Value.SelectMany(s => s.ToPointsFeatures()).ToList()),
            _ => throw new Exception(),
        };
    }

    public static Dictionary<int, List<IFeature>> ToFeatureVertices(this TrackBuilderResult result)
    {
        return result.Track
            .ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => s.ToPointsFeatures()).ToList());
    }

    public static List<IFeature> ToPointsFeatures(this List<(double lonDeg, double latDeg)> list)
    {
        return list
            .Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg))
            .ToCoordinates()
            .Select(s => new GeometryFactory().CreatePoint(s))
            .Select(s => (IFeature)s.ToFeature())
            .ToList();
    }
}
