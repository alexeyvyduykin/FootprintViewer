using FootprintViewer.Data.Models;
using FootprintViewer.ViewModels.SidePanel.Items;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public sealed class GroundTargetTabFilterViewModel : AOIFilterViewModel<GroundTargetViewModel>
{
    private const bool IsAreaDefault = true;
    private const bool IsRouteDefault = true;
    private const bool IsPointDefault = true;

    public GroundTargetTabFilterViewModel(IReadonlyDependencyResolver _)
    {
        IsArea = true;
        IsRoute = true;
        IsPoint = true;

        var observable1 = this.WhenAnyValue(s => s.IsArea, s => s.IsRoute, s => s.IsPoint)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        SetMergeObservables(new[] { observable1 });
        SetDirtyMergeObservables(new[] { observable1 });

        this.WhenAnyValue(s => s.IsArea, s => s.IsRoute, s => s.IsPoint, (s1, s2, s3) => new[] { s1, s2, s3 })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => s.AllPropertyCheck())
            .Subscribe(s => IsAllTypes = s);

        this.WhenAnyValue(s => s.IsAllTypes)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Select(s => (bool)s!)
            .Subscribe(s => IsArea = IsRoute = IsPoint = s);
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

    protected override bool AOIFiltering(GroundTargetViewModel groundTarget)
    {
        if (IsAOIActive == true && AOI is Polygon aoiPoly)
        {
            var geometry = groundTarget.Geometry;

            if (geometry is Point point)
            {
                return aoiPoly.Contains(point);
            }
            else if (geometry is LineString lineString)
            {
                return aoiPoly.Intersection(lineString.ToLinearPolygon(), IsFullCoverAOI);
            }
            else if (geometry is Polygon polygon)
            {
                return aoiPoly.Intersection(polygon, IsFullCoverAOI);
            }
        }

        return true;
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

    [Reactive]
    public bool? IsAllTypes { get; set; }

    [Reactive]
    public bool IsArea { get; set; }

    [Reactive]
    public bool IsRoute { get; set; }

    [Reactive]
    public bool IsPoint { get; set; }
}
