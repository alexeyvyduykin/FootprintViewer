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
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public class FootprintTabFilterViewModel : BaseFilterViewModel<FootprintViewModel>
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<SatelliteItemViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteItemViewModel> _items;
    private readonly IObservable<Func<FootprintViewModel, bool>> _filterObservable;

    public FootprintTabFilterViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        IsLeftStrip = true;
        IsRightStrip = true;
        FromNode = 1;
        ToNode = 15;

        _dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateImpl));

        var _activeChanged = _satellites.Connect().WhenValueChanged(p => p.IsActive);

        var observable1 = this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftStrip, s => s.IsRightStrip)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable2 = _activeChanged
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var merged = Observable.Merge(observable1, observable2);

        _filterObservable = merged.Select(CreatePredicate);

        Observable.StartAsync(UpdateImpl, RxApp.MainThreadScheduler).Subscribe();
    }

    public override IObservable<Func<FootprintViewModel, bool>> FilterObservable => _filterObservable;

    private static Func<FootprintViewModel, bool> CreatePredicate(FootprintTabFilterViewModel filter)
    {
        return footprint =>
        {
            if (filter.Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
            {
                if (footprint.Node >= filter.FromNode && footprint.Node <= filter.ToNode)
                {
                    if (footprint.Direction == SatelliteStripDirection.Left && filter.IsLeftStrip == true)
                    {
                        return true;
                    }

                    if (footprint.Direction == SatelliteStripDirection.Right && filter.IsRightStrip == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        };
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

    public override bool Filtering(FootprintViewModel footprint)
    {
        if (Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
        {
            if (footprint.Node >= FromNode && footprint.Node <= ToNode)
            {
                if (footprint.Direction == SatelliteStripDirection.Left && IsLeftStrip == true)
                {
                    return true;
                }

                if (footprint.Direction == SatelliteStripDirection.Right && IsRightStrip == true)
                {
                    return true;
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
    public bool IsLeftStrip { get; set; }

    [Reactive]
    public bool IsRightStrip { get; set; }

    public ReadOnlyObservableCollection<SatelliteItemViewModel> Satellites => _items;

    // TODO: after remove FootprintPreview, replace SetAoi() to this
    [Reactive]
    public Geometry? AOI { get; set; }

    public override string[]? Names => throw new NotImplementedException();
}
