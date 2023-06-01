using FootprintViewer;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Helpers;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using ReactiveUI.Fody.Helpers;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Map = new Map();
        Map.AddLayer(new Layer(), LayerType.WorldMap);

        string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");
        var resource = new MapResource("world", path);
        Map.SetWorldMapLayer(resource);

        PlannedScheduleTab = new();
    }

    public PlannedScheduleTabViewModel PlannedScheduleTab { get; set; }

    public Map Map { get; private set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;
}
