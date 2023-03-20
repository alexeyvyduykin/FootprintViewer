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

        var layer = new WritableLayer();
        var pointsLayer1 = new WritableLayer() { Style = CreatePointsStyle(Color.Red) };
        var pointsLayer2 = new WritableLayer() { Style = CreatePointsStyle(Color.Blue) };

        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);
        var nodes = satellite.Nodes().Count;
        var track1 = new FactorTrack22(orbit);
        var track2 = new FactorTrack22(orbit);

        track1.CalculateTrackWithLogStep(100);
        track2.CalculateTrack(60);

        for (int i = 0; i < nodes; i++)
        {
            var offsetDeg = track1.NodeOffsetDeg;
            var list = track1.CacheTrack.Select(s => (s.lonDeg + offsetDeg * i, s.latDeg)).ToList();

            AddTrack(list, layer);
            AddPoints(list, pointsLayer1);
        }

        var list2 = track2.CacheTrack.Select(s => (s.lonDeg + 5.0, s.latDeg)).ToList();

        AddTrack(list2, layer);
        AddPoints(list2, pointsLayer2);

        map.Layers.Add(layer);
        map.Layers.Add(pointsLayer1);
        map.Layers.Add(pointsLayer2);

        return map;
    }

    private static void AddTrack(IList<(double lonDeg, double latDeg)> cache, WritableLayer layer)
    {
        var vertices0 = cache.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var line0 = new GeometryFactory().CreateLineString(vertices0.ToCoordinates());

        layer.Add((IFeature)line0.ToFeature());
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

