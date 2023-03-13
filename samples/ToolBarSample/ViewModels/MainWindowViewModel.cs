using ReactiveUI.Fody.Helpers;
using System;
using ToolBarSample.Models;

namespace ToolBarSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _consoleString = string.Empty;

    public MainWindowViewModel()
    {
        var mapState = new MapState();

        ToolBar = new ToolBarViewModel(mapState);

        mapState.ResetObservable.Subscribe(_ => Print(mapState));

        mapState.SelectObservable.Subscribe(_ => Print(mapState));

        mapState.RectAOIObservable.Subscribe(_ => Print(mapState));

        mapState.CircleAOIObservable.Subscribe(_ => Print(mapState));

        mapState.PolygonAOIObservable.Subscribe(_ => Print(mapState));

        mapState.RouteObservable.Subscribe(_ => Print(mapState));

        mapState.TranslateObservable.Subscribe(_ => Print(mapState));

        mapState.ScaleObservable.Subscribe(_ => Print(mapState));

        mapState.RotateObservable.Subscribe(_ => Print(mapState));

        mapState.EditObservable.Subscribe(_ => Print(mapState));

        mapState.PointObservable.Subscribe(_ => Print(mapState));

        mapState.RectObservable.Subscribe(_ => Print(mapState));

        mapState.CircleObservable.Subscribe(_ => Print(mapState));

        mapState.PolygonObservable.Subscribe(_ => Print(mapState));
    }

    [Reactive]
    public ToolBarViewModel ToolBar { get; set; }

    [Reactive]
    public string ConsoleString { get; set; } = "Console";

    private void WriteLine(string str)
    {
        _consoleString += str + "\n";

        ConsoleString = _consoleString;
    }

    private void Print(MapState mapState)
    {
        WriteLine($"[State:] {mapState.State}");
    }
}
