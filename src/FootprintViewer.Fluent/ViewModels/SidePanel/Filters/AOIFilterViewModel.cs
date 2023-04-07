using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Filters;

public abstract class AOIFilterViewModel<T> : BaseFilterViewModel<T>, IAOIFilter<T> where T : ViewModelBase
{
    private readonly IObservable<Func<T, bool>> _aoiFilterObservable;

    public AOIFilterViewModel()
    {
        IsAOIActive = true;
        IsFullCoverAOI = false;

        var observable1 = this.WhenAnyValue(s => s.IsAOIActive, s => s.IsFullCoverAOI)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable2 = this.WhenAnyValue(s => s.AOI)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => this);

        var merged = Observable.Merge(observable1, observable2);

        _aoiFilterObservable = merged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(CreateAOIPredicate);
    }

    private static Func<T, bool> CreateAOIPredicate(AOIFilterViewModel<T> filter)
    {
        return s => filter.AOIFiltering(s);
    }

    protected abstract bool AOIFiltering(T viewModel);

    public IObservable<Func<T, bool>> AOIFilterObservable => _aoiFilterObservable;

    [Reactive]
    public Geometry? AOI { get; set; }

    [Reactive]
    public bool IsFullCoverAOI { get; set; }

    [Reactive]
    public bool IsAOIActive { get; set; }
}
