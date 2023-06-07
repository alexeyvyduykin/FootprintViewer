using BruTile.MbTiles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using SQLite;
using System;
using System.Runtime.InteropServices;

namespace SpaceScienceTest.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel Instance = new();

    public const string WorldKey = "WorldMapLayer";

    public MainWindowViewModel()
    {
        Map = new Map();

        Map.Layers.Add(CreateWorldMapLayer());
    }

    private static ILayer CreateWorldMapLayer()
    {
        var fullBaseDirectory = System.IO.Path.GetFullPath(AppContext.BaseDirectory);

        string path = System.IO.Path.Combine(GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource)
        {
            Name = WorldKey
        };
    }

    public static string GetFullBaseDirectory()
    {
        var fullBaseDirectory = System.IO.Path.GetFullPath(AppContext.BaseDirectory);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!fullBaseDirectory.StartsWith('/'))
            {
                fullBaseDirectory = fullBaseDirectory.Insert(0, "/");
            }
        }

        return fullBaseDirectory;
    }

    public Map Map { get; private set; }
}
