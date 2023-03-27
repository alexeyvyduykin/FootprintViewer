using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using ReactiveUI.Fody.Helpers;
using SpaceScienceSample.Models;
using System;
using System.Collections.Generic;

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

        double targetLonDeg = 178.0;
        double targetLatDeg = 70.0;

        Values = factory.CreatePlotValues(targetLonDeg, targetLatDeg);

        Asymptotes = factory.CreatePlotAsymptotes();

        (Points1, Info1) = factory.Method1(targetLonDeg, targetLatDeg);
        (Points2, Info2) = factory.Method2(targetLonDeg, targetLatDeg);

        MapNavigator.ClickObservable.Subscribe(s =>
        {
            Values = factory.CreatePlotValues(s.lonDeg, s.latDeg);

            (Points1, Info1) = factory.Method1(s.lonDeg, s.latDeg);

            (Points2, Info2) = factory.Method2(s.lonDeg, s.latDeg);
        });
    }

    public Map Map => _map;

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    [Reactive]
    public IMapNavigator MapNavigator { get; set; }

    [Reactive]
    public List<PlotItem> Values { get; set; }

    [Reactive]
    public List<PlotItem> Asymptotes { get; set; }

    [Reactive]
    public List<PlotPoint> Points1 { get; set; }

    [Reactive]
    public string Info1 { get; set; }

    [Reactive]
    public List<PlotPoint> Points2 { get; set; }

    [Reactive]
    public string Info2 { get; set; }
}
