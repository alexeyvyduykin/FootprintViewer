using FootprintViewer.Data;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public class GroundTargetTabFilterViewModel : ViewModelBase, IFilter<GroundTargetViewModel>
{
    private readonly IObservable<Func<GroundTargetViewModel, bool>> _filterObservable;
    private readonly ObservableAsPropertyHelper<bool> _isDirty;

    private const bool IsAreaDefault = true;
    private const bool IsRouteDefault = true;
    private const bool IsPointDefault = true;

    public GroundTargetTabFilterViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        IsArea = true;
        IsRoute = true;
        IsPoint = true;

        this.WhenAnyValue(s => s.IsArea, s => s.IsRoute, s => s.IsPoint)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => TypesChangedImpl());

        var observable = this.WhenAnyValue(s => s.IsArea, s => s.IsRoute, s => s.IsPoint)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        _filterObservable = observable.Select(CreatePredicate);

        this.WhenAnyValue(s => s.IsAllTypes)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(IsAllTypesChanged);

        var dirty = observable;

        var obs1 = observable
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => IsNotDefault(s));

        Reset = ReactiveCommand.Create(ResetImpl, outputScheduler: RxApp.MainThreadScheduler);

        var obs2 = Reset.Select(_ => false);

        _isDirty = Observable.Merge(obs1, obs2)
            .ToProperty(this, x => x.IsDirty);
    }

    public IObservable<Func<GroundTargetViewModel, bool>> FilterObservable => _filterObservable;

    private static Func<GroundTargetViewModel, bool> CreatePredicate(GroundTargetTabFilterViewModel filter)
    {
        return s => filter.Filtering(s);
    }

    private bool Filtering(GroundTargetViewModel groundTarget)
    {
        var type = groundTarget.Type;

        if ((type == GroundTargetType.Area && IsArea == true)
            || (type == GroundTargetType.Route && IsRoute == true)
            || (type == GroundTargetType.Point && IsPoint == true))
        {
            return true;
        }

        return false;
    }

    private void TypesChangedImpl()
    {
        var allTrue = IsArea && IsRoute && IsPoint;
        var allFalse = !IsArea && !IsRoute && !IsPoint;
        var anyFalse = !IsArea || !IsRoute || !IsPoint;

        if (allTrue == true)
        {
            IsAllTypes = true;
        }
        else if (allFalse == true)
        {
            IsAllTypes = false;
        }
        else if (anyFalse == true)
        {
            IsAllTypes = null;
        }
    }

    private static bool IsNotDefault(GroundTargetTabFilterViewModel filter)
    {
        if (IsAreaDefault == filter.IsArea
            && IsRouteDefault == filter.IsRoute
            && IsPointDefault == filter.IsPoint)
        {
            return false;
        }

        return true;
    }

    private void ResetImpl()
    {
        IsArea = IsAreaDefault;
        IsRoute = IsRouteDefault;
        IsPoint = IsPointDefault;
    }

    private void IsAllTypesChanged(bool? value)
    {
        if (value is bool available)
        {
            IsArea = available;
            IsRoute = available;
            IsPoint = available;
        }
    }

    [Reactive]
    public bool? IsAllTypes { get; set; }

    [Reactive]
    public bool IsArea { get; set; }

    [Reactive]
    public bool IsRoute { get; set; }

    [Reactive]
    public bool IsPoint { get; set; }

    [Reactive]
    public bool IsFullCoverAOI { get; set; }

    public ReactiveCommand<Unit, Unit> Reset { get; }

    public bool IsDirty => _isDirty.Value;
}
