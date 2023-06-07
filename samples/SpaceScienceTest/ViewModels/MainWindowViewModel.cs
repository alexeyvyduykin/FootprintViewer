using BruTile.MbTiles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using SpaceScienceTest.Layers;
using SpaceScienceTest.ViewModels.Tests;
using SQLite;
using System;

namespace SpaceScienceTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel Instance = new();

    public const string WorldKey = "WorldMapLayer";
    public const string TestKey = "TestLayer";

    public MainWindowViewModel()
    {
        Map = new Map();

        Map.Layers.Add(CreateWorldMapLayer());
        Map.Layers.Add(CreateTestLayer());

        GroundTrackTest = new(this);
    }

    private static ILayer CreateWorldMapLayer()
    {
        var fullBaseDirectory = System.IO.Path.GetFullPath(AppContext.BaseDirectory);

        string path = System.IO.Path.Combine(fullBaseDirectory, "Assets", "world.mbtiles");

        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource)
        {
            Name = WorldKey
        };
    }

    private static ILayer CreateTestLayer()
    {
        return new QueueLayer()
        {
            Name = TestKey,
            Style = CreateTestLayerStyle()
        };
    }

    private static IStyle CreateTestLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            if (gf.Geometry is Point)
            {
                return null;
            }

            if ((string)gf["Name"]! == "Arrow")
            {
                return new VectorStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Red, 1.0f)),
                    Outline = new Pen(Color.Red, 1.0),
                    Line = new Pen(Color.Red, 1.0)
                };
            }

            return new VectorStyle()
            {
                Fill = new Brush(Color.Opacity(Color.Green, 1.0f)),
                Line = new Pen(Color.Green, 2.0),
                Outline = new Pen(Color.Green, 2.0),
            };
        });
    }


    public GroundTrackTestViewModel GroundTrackTest { get; set; }

    public Map Map { get; private set; }
}
