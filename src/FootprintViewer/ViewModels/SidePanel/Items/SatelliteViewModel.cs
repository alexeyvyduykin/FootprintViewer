using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Items;

public class SatelliteViewModel : ViewModelBase, IViewerItem
{
    private readonly string _name;
    private readonly Satellite _satellite;
    private readonly int _minNode = 1;
    private readonly int _maxNode;
    private readonly IObservable<SatelliteViewModel> _trackObservable;
    private readonly IObservable<SatelliteViewModel> _stripsObservable;

    public SatelliteViewModel(Satellite satellite)
    {
        _satellite = satellite;

        _name = satellite.Name!;

        var count = satellite.ToPRDCTSatellite().Nodes().Count;

        _maxNode = count;

        CurrentNode = 1;

        _trackObservable = this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsTrack)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);

        _stripsObservable = this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsLeftStrip, s => s.IsRightStrip)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);
    }

    public IObservable<SatelliteViewModel> TrackObservable => _trackObservable;

    public IObservable<SatelliteViewModel> StripsObservable => _stripsObservable;

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
    public bool IsLeftStrip { get; set; } = true;

    [Reactive]
    public bool IsRightStrip { get; set; } = false;
}
