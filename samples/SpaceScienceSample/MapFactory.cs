using Avalonia.Controls;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using SpaceScience;
using SpaceScience.Model;
using SpaceScienceSample.Extensions;
using SpaceScienceSample.Models;
using SQLite;
using System.Collections.Generic;

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
        var swaths = satellite.BuildSwaths(40, 16);

        var features = tracks.ToFeature("Satellite1");
        var leftFeatures = swaths.ToFeature("SatelliteLeft1", SwathMode.Left);
        var rightFeatures = swaths.ToFeature("SatelliteRight1", SwathMode.Right);

        var trackVertices = tracks.ToFeatureVertices();
        var leftVertices = swaths.ToFeatureVertices(SwathMode.Left);
        var rightVertices = swaths.ToFeatureVertices(SwathMode.Right);

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

        AddFeatures(trackVertices[0], pointsLayer1);
        AddFeatures(trackVertices[1], pointsLayer1);

        AddFeatures(leftVertices[0], pointsLayer2);
        AddFeatures(rightVertices[0], pointsLayer2);
        AddFeatures(leftVertices[1], pointsLayer2);
        AddFeatures(rightVertices[1], pointsLayer2);

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
        var track1 = new GroundTrack(orbit);

        track1.CalculateTrackWithLogStep(100);

        for (int i = 0; i < nodes; i++)
        {
            var list = track1.GetTrack(i);

            AddFeatures(new() { list.ToLineStringFeature(string.Empty) }, layer);
            AddFeatures(list.ToPointsFeatures(), pointsLayer1);
        }

        map.Layers.Add(layer);
        map.Layers.Add(pointsLayer1);
    }

    private static void AddFeatures(List<IFeature> features, WritableLayer layer)
    {
        layer.AddRange(features);
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