using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace PlannedScheduleOnMapSample.ViewModels.Items;

public class SatelliteViewModel : ViewModelBase
{
    private readonly IObservable<SatelliteViewModel> _observable;

    public SatelliteViewModel() : this(SatelliteBuilder.CreateRandom()) { }

    public SatelliteViewModel(Satellite satellite)
    {
        Satellite = satellite;

        Name = satellite.Name;

        Node = 1;

        _observable = this.WhenAnyValue(s => s.IsVisible, s => s.Node)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);
    }

    public IObservable<SatelliteViewModel> Observable => _observable;

    public Satellite Satellite { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public int Node { get; set; }

    public string Name { get; set; }
}
