using Avalonia.Controls;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using ReactiveUI.Fody.Helpers;
using SpaceScienceSample.Models;
using SQLite;

namespace SpaceScienceSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Map _map;
    private readonly ScaleMapBar _scaleMapBar;

    private static ILayer CreateWorldMapLayer(string path)
    {
        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource);
    }
    public MainWindowViewModel()
    {
        _map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        var path = @"..\FootprintViewer\data\world\world.mbtiles";

        if (Design.IsDesignMode == false)
        {
            path = @"..\..\..\..\..\" + path;
        }

        _map.Layers.Add(CreateWorldMapLayer(path));

        MapNavigator = new MapNavigator(_map);

        _scaleMapBar = new ScaleMapBar();
    }

    public Map Map => _map;

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    [Reactive]
    public IMapNavigator MapNavigator { get; set; }
}
