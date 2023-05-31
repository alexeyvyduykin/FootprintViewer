using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.SidePanel.Filters;

public abstract class AOIFilterViewModel<T> : BaseFilterViewModel<T>, IAOIFilter<T> where T : ViewModelBase
{
    private readonly IObservable<Func<T, bool>> _aoiObservable;

    public AOIFilterViewModel()
    {
        IsAOIActive = true;
        IsFullCoverAOI = false;

        var observable1 = this.WhenAnyValue(s => s.IsAOIActive, s => s.IsFullCoverAOI)
            .ObserveOn(RxApp.MainThreadScheduler)
            // TODO: skip init?
            //.Skip(2)
            .Throttle(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(_ => this);

        var observable2 = this.WhenAnyValue(s => s.AOI)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(_ => this);

        var merged = Observable.Merge(observable1, observable2);

        _aoiObservable = merged
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(CreateAOIPredicate);
    }

    public void SetAOIObservable(IObservable<Geometry?> observable)
    {
        observable
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Subscribe(s => AOI = s);
    }

    private static Func<T, bool> CreateAOIPredicate(AOIFilterViewModel<T> filter)
    {
        return s => filter.AOIFiltering(s);
    }

    protected abstract bool AOIFiltering(T viewModel);

    public IObservable<Func<T, bool>> AOIObservable => _aoiObservable;

    [Reactive]
    protected Geometry? AOI { get; set; }

    [Reactive]
    public bool IsFullCoverAOI { get; set; }

    [Reactive]
    public bool IsAOIActive { get; set; }
}
