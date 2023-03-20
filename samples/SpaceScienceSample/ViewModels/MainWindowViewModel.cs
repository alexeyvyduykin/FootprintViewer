using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using ReactiveUI.Fody.Helpers;
using SpaceScienceSample.Models;

namespace SpaceScienceSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Map _map;
    private readonly ScaleMapBar _scaleMapBar;

    public MainWindowViewModel()
    {
        var factory = new MapFactory();

        _map = factory.CreateMap();

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
