using Avalonia.Controls;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using SpaceScience;
using SpaceScience.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample;

internal class MapFactory
{
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheTrack = new();
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheLeft = new();
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheRight = new();

    public MapFactory()
    {

    }

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

        add(layer);

        map.Layers.Add(layer);

        return map;
    }

    private void add(WritableLayer layer)
    {
        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);

        Cache(satellite);

        var list0 = _cacheTrack[0].Select(s => ((lonDeg: s.lon * SpaceMath.RadiansToDegrees, latDeg: s.lat * SpaceMath.RadiansToDegrees)));
        var list1 = _cacheLeft[0].Select(s => ((lonDeg: s.lon * SpaceMath.RadiansToDegrees, latDeg: s.lat * SpaceMath.RadiansToDegrees)));
        var list2 = _cacheRight[0].Select(s => ((lonDeg: s.lon * SpaceMath.RadiansToDegrees, latDeg: s.lat * SpaceMath.RadiansToDegrees)));

        var vertices0 = list0.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));
        var vertices1 = list1.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));
        var vertices2 = list2.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var line0 = new GeometryFactory().CreateLineString(vertices0.ToCoordinates());
        var line1 = new GeometryFactory().CreateLineString(vertices1.ToCoordinates());
        var line2 = new GeometryFactory().CreateLineString(vertices2.ToCoordinates());

        var point = new GeometryFactory().CreatePoint(vertices0.ToCoordinates().First());

        layer.Add((IFeature)line0.ToFeature());
        //layer.Add((IFeature)line1.ToFeature());
        //layer.Add((IFeature)line2.ToFeature());
        layer.Add((IFeature)point.ToFeature());
    }

    private void Cache(PRDCTSatellite satellite, double angle = 20.0, double dt = 1.0)
    {
        _cacheTrack.Clear();
        _cacheLeft.Clear();
        _cacheRight.Clear();

        var orbit = satellite.Orbit;
        var nodeCount = satellite.Nodes().Count;
        var period = orbit.Period;

        for (int i = 0; i < nodeCount; i++)
        {
            _cacheTrack.Add(i, new List<(double t, double u, double lon, double lat)>());
            _cacheLeft.Add(i, new List<(double t, double u, double lon, double lat)>());
            _cacheRight.Add(i, new List<(double t, double u, double lon, double lat)>());

            for (double t = 0; t <= period; t += dt)
            {
                var u = orbit.Anomalia(t);

                var (lon, lat) = new CustomTrack(satellite.Orbit, 0.0, TrackPointDirection.None)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                var (lonLeft, latLeft) = new CustomTrack(satellite.Orbit, angle, TrackPointDirection.Left)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                var (lonRight, latRight) = new CustomTrack(satellite.Orbit, angle, TrackPointDirection.Right)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                _cacheTrack[i].Add((t, u, lon, lat));
                _cacheLeft[i].Add((t, u, lonLeft, latLeft));
                _cacheRight[i].Add((t, u, lonRight, latRight));
            }
        }

        return;
    }

    private static bool MinLon(double lon) => lon < -Math.PI;

    private static bool MaxLon(double lon) => lon > Math.PI;

    private static ILayer CreateWorldMapLayer(string path)
    {
        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource);
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