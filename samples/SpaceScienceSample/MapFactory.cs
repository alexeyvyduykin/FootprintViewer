using Avalonia.Controls;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using SpaceScience;
using SpaceScienceSample.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample;

internal class MapFactory
{
    public Map CreateMap()
    {
        var map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        var path = @"..\FootprintViewer\data\world\world.mbtiles";

        if (Design.IsDesignMode == false)
        {
            path = @"..\..\..\..\..\" + path;
        }

        map.Layers.Add(CreateWorldMapLayer(path));

        //Sample1(map);
        Sample2(map);

        return map;
    }

    private void Sample2(Map map)
    {
        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);

        var tracks = satellite.BuildTracks();
        var res2 = SpaceScienceBuilder2.BuildSwaths(satellite, 40, 16);

        var features = FeatureBuilder.BuildTrack("Satellite1", tracks);
        var leftFeatures = FeatureBuilder.Build("SatelliteLeft1", res2.Left);
        var rightFeatures = FeatureBuilder.Build("SatelliteRight1", res2.Right);


        var layer1 = new WritableLayer();
        var layer2 = new WritableLayer() { Style = CreateSwathStyle(Color.Green) };
        var pointsLayer1 = new WritableLayer() { Style = CreatePointsStyle(Color.Red) };
        var pointsLayer2 = new WritableLayer() { Style = CreatePointsStyle(Color.Blue) };

        AddFeatures(features[0], layer1);
        AddFeatures(features[1], layer1);

        AddFeatures(leftFeatures[0], layer2);
        AddFeatures(rightFeatures[0], layer2);
        AddFeatures(leftFeatures[1], layer2);
        AddFeatures(rightFeatures[1], layer2);

        AddPoints(tracks[0], pointsLayer1);
        AddPoints(tracks[1], pointsLayer1);

        AddPoints(res2.Left[0], pointsLayer2);
        AddPoints(res2.Right[0], pointsLayer2);
        AddPoints(res2.Left[1], pointsLayer2);
        AddPoints(res2.Right[1], pointsLayer2);

        map.Layers.Add(layer1);
        map.Layers.Add(layer2);
        map.Layers.Add(pointsLayer1);
        map.Layers.Add(pointsLayer2);
    }

    private void Sample1(Map map)
    {
        var layer = new WritableLayer();
        var pointsLayer1 = new WritableLayer() { Style = CreatePointsStyle(Color.Red) };

        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);
        var nodes = satellite.Nodes().Count;
        var track1 = new FactorTrack22(orbit);

        track1.CalculateTrackWithLogStep(100);

        for (int i = 0; i < nodes; i++)
        {
            var list = track1.GetTrack(i);

            AddTrack(list, layer);
            AddPoints(list, pointsLayer1);
        }

        map.Layers.Add(layer);
        map.Layers.Add(pointsLayer1);
    }

    private static void AddTrack(IList<(double lonDeg, double latDeg)> cache, WritableLayer layer)
    {
        var vertices0 = cache.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var line0 = new GeometryFactory().CreateLineString(vertices0.ToCoordinates());

        layer.Add((IFeature)line0.ToFeature());
    }

    private static void AddFeatures(List<IFeature> features, WritableLayer layer)
    {
        layer.AddRange(features);
    }

    private static void AddPoints(List<(double lonDeg, double latDeg)> cache, WritableLayer layer)
    {
        var vertices0 = cache.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        foreach (var item in vertices0.ToCoordinates())
        {
            var point = new GeometryFactory().CreatePoint(item);
            layer.Add((IFeature)point.ToFeature());
        }
    }

    public static void AddPoints(List<List<(double lon, double lat)>> tracks, WritableLayer layer)
    {
        var vertices0 = tracks.SelectMany(s => s.Select(t => SphericalMercator.FromLonLat(t.lon, t.lat))).ToList();

        foreach (var item in vertices0.ToCoordinates())
        {
            var point = new GeometryFactory().CreatePoint(item);
            layer.Add((IFeature)point.ToFeature());
        }
    }

    private static ILayer CreateWorldMapLayer(string path)
    {
        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource);
    }

    private static SymbolStyle CreatePointsStyle(Color color)
    {
        return new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            Fill = new Brush(color),
            SymbolScale = 0.20,
        };
    }

    private static IStyle CreateSwathStyle(Color color)
    {
        return new VectorStyle
        {
            Fill = new Brush(color),
            Opacity = 0.4f,
        };
    }
}

public static class Class1Extensions
{
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

