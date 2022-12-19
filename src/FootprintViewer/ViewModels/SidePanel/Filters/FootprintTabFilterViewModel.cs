using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.SidePanel.Items;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public class FootprintTabFilterViewModel : ViewModelBase, IFilter<FootprintViewModel>
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<SatelliteItemViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteItemViewModel> _items;
    private readonly IObservable<Func<FootprintViewModel, bool>> _filterObservable;
    private readonly ObservableAsPropertyHelper<bool> _isDirty;

    private const bool IsLeftSwathDefault = true;
    private const bool IsRightSwathDefault = true;
    private const int FromNodeDefault = 1;
    private const int ToNodeDefault = 15;

    public FootprintTabFilterViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        IsLeftSwath = true;
        IsRightSwath = true;
        FromNode = 1;
        ToNode = 15;

        _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Footprints.ToString()))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateImpl));

        var _activeChanged = _satellites.Connect().WhenValueChanged(p => p.IsActive);

        var observable1 = this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftSwath, s => s.IsRightSwath)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable2 = _activeChanged
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable3 = this.WhenAnyValue(s => s.AOI)
            .Select(_ => this);

        var merged = Observable.Merge(observable1, observable2, observable3);
        var dirtyMerged = Observable.Merge(observable1, observable2);

        _filterObservable = merged.Select(CreatePredicate);

        var obs1 = dirtyMerged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => IsNotDefault(s));

        Reset = ReactiveCommand.Create(ResetImpl, outputScheduler: RxApp.MainThreadScheduler);

        var obs2 = Reset.Select(_ => false);

        _isDirty = Observable.Merge(obs1, obs2)
            .ToProperty(this, x => x.IsDirty);

        Observable.StartAsync(UpdateImpl, RxApp.MainThreadScheduler);
    }

    public IObservable<Func<FootprintViewModel, bool>> FilterObservable => _filterObservable;

    private static Func<FootprintViewModel, bool> CreatePredicate(FootprintTabFilterViewModel filter)
    {
        return s => filter.Filtering(s);
    }

    private void ResetImpl()
    {
        IsLeftSwath = IsLeftSwathDefault;
        IsRightSwath = IsRightSwathDefault;
        FromNode = FromNodeDefault;
        ToNode = ToNodeDefault;

        foreach (var item in Satellites)
        {
            item.IsActive = true;
        }
    }

    private static bool IsNotDefault(FootprintTabFilterViewModel filter)
    {
        if (IsLeftSwathDefault == filter.IsLeftSwath
            && IsRightSwathDefault == filter.IsRightSwath
            && FromNodeDefault == filter.FromNode
            && ToNodeDefault == filter.ToNode)
        {
            return filter.Satellites.Any(s => s.IsActive == false);
        }

        return true;
    }

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

        var satellites = res
            .Where(s => string.IsNullOrEmpty(s.SatelliteName) == false)
            .Select(s => s.SatelliteName!)
            .Distinct()
            .OrderBy(s => s)
            .Select(s => new SatelliteItemViewModel() { Name = s })
            .ToList();

        _satellites.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(satellites);
        });
    }

    private bool Filtering(FootprintViewModel footprint)
    {
        bool isAoiCondition = false;

        if (AOI == null)
        {
            isAoiCondition = true;
        }
        else
        {
            if (footprint.Polygon is Polygon polygon)
            {
                var aoiPolygon = (Polygon)AOI;

                isAoiCondition = aoiPolygon.Intersection(polygon, true);
            }
        }

        if (isAoiCondition == true)
        {
            if (Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
            {
                if (footprint.Node >= FromNode && footprint.Node <= ToNode)
                {
                    if (footprint.Direction == SwathDirection.Left && IsLeftSwath == true)
                    {
                        return true;
                    }

                    if (footprint.Direction == SwathDirection.Right && IsRightSwath == true)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [Reactive]
    public int FromNode { get; set; }

    [Reactive]
    public int ToNode { get; set; }

    [Reactive]
    public bool IsLeftSwath { get; set; }

    [Reactive]
    public bool IsRightSwath { get; set; }

    public ReadOnlyObservableCollection<SatelliteItemViewModel> Satellites => _items;

    [Reactive]
    public Geometry? AOI { get; set; }

    public ReactiveCommand<Unit, Unit> Reset { get; }

    public bool IsDirty => _isDirty.Value;
}
