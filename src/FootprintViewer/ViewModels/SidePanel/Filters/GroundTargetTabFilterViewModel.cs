using FootprintViewer.Data;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public class GroundTargetTabFilterViewModel : BaseFilterViewModel<GroundTargetViewModel>
{
    private const bool IsAreaDefault = true;
    private const bool IsRouteDefault = true;
    private const bool IsPointDefault = true;

    public GroundTargetTabFilterViewModel(IReadonlyDependencyResolver _)
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

        this.WhenAnyValue(s => s.IsAllTypes)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(IsAllTypesChanged);

        SetMergeObservables(new[] { observable });
        SetDirtyMergeObservables(new[] { observable });
    }

    protected override bool Filtering(GroundTargetViewModel groundTarget)
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

    protected override bool IsDefaultImpl()
    {
        if (IsAreaDefault == IsArea
            && IsRouteDefault == IsRoute
            && IsPointDefault == IsPoint)
        {
            return true;
        }

        return false;
    }

    protected override void ResetImpl()
    {
        IsArea = IsAreaDefault;
        IsRoute = IsRouteDefault;
        IsPoint = IsPointDefault;
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
}
