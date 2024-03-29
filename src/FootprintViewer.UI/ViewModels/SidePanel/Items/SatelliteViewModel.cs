﻿using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using Mapsui.Styles;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.SidePanel.Items;

public sealed class SatelliteViewModel : ViewModelBase, IViewerItem
{
    private readonly string _name;
    private readonly Satellite _satellite;
    private readonly int _minNode = 1;
    private readonly int _maxNode;
    private readonly IObservable<SatelliteViewModel> _trackObservable;
    private readonly IObservable<SatelliteViewModel> _swathsObservable;

    public SatelliteViewModel() : this(SatelliteBuilder.CreateRandom()) { }

    public SatelliteViewModel(Satellite satellite)
    {
        _satellite = satellite;

        _name = satellite.Name!;

        var nodes = satellite.NodesOnDay();

        _maxNode = nodes;

        CurrentNode = 1;

        _trackObservable = this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsTrack)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);

        _swathsObservable = this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsLeftSwath, s => s.IsRightSwath)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);
    }

    public IObservable<SatelliteViewModel> TrackObservable => _trackObservable;

    public IObservable<SatelliteViewModel> SwathsObservable => _swathsObservable;

    public string Name => _name;

    public Satellite Satellite => _satellite;

    public int MinNode => _minNode;

    public int MaxNode => _maxNode;

    [Reactive]
    public int CurrentNode { get; set; }

    [Reactive]
    public bool IsShow { get; set; } = false;

    [Reactive]
    public bool IsShowInfo { get; set; } = false;

    [Reactive]
    public bool IsTrack { get; set; } = true;

    [Reactive]
    public bool IsLeftSwath { get; set; } = true;

    [Reactive]
    public bool IsRightSwath { get; set; } = false;

    [Reactive]
    public Color? Color { get; set; }
}
