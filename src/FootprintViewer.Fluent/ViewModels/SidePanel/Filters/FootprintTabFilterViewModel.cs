﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using FootprintViewer.Services;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.SidePanel.Filters;

public sealed class FootprintTabFilterViewModel : AOIFilterViewModel<FootprintViewModel>
{
    private readonly ILocalStorageService _localStorage;
    private readonly SourceList<SatelliteItemViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteItemViewModel> _items;

    private const bool IsLeftSwathDefault = true;
    private const bool IsRightSwathDefault = true;
    private const int FromNodeDefault = 1;
    private const int ToNodeDefault = 15;

    public FootprintTabFilterViewModel()
    {
        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        IsLeftSwath = true;
        IsRightSwath = true;
        FromNode = 1;
        ToNode = 15;

        _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateImpl));

        var _activeChanged = _satellites
            .Connect()
            .WhenValueChanged(p => p.IsActive);

        var observable1 = this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftSwath, s => s.IsRightSwath)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable2 = _activeChanged
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        SetMergeObservables(new[] { observable1, observable2 });
        SetDirtyMergeObservables(new[] { observable1, observable2 });

        _items
            .ToObservableChangeSet()
            .WhenPropertyChanged(p => p.IsActive)
            .Select(_ => Satellites.AllPropertyCheck(p => p.IsActive))
            .Subscribe(s => IsAllSatellites = s);

        this.WhenAnyValue(s => s.IsAllSatellites)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Select(s => (bool)s!)
            .Subscribe(value => Satellites.SetValue(s => s.IsActive = value));

        Observable.StartAsync(UpdateImpl, RxApp.MainThreadScheduler);
    }

    protected override void ResetImpl()
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

    protected override bool IsDefaultImpl()
    {
        if (IsLeftSwathDefault == IsLeftSwath
            && IsRightSwathDefault == IsRightSwath
            && FromNodeDefault == FromNode
            && ToNodeDefault == ToNode)
        {
            return Satellites.All(s => s.IsActive == true);
        }

        return false;
    }

    protected override bool Filtering(FootprintViewModel footprint)
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

        return false;
    }

    protected override bool AOIFiltering(FootprintViewModel footprint)
    {
        if (IsAOIActive == true && AOI is Polygon aoiPolygon)
        {
            if (footprint.Polygon is Polygon polygon)
            {
                return aoiPolygon.Intersection(polygon, IsFullCoverAOI);
            }
        }

        return true;
    }

    private async Task UpdateImpl()
    {
        var ps = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            var satellites = ps.Satellites
                .OrderBy(s => s.Name)
                .Select(s => new SatelliteItemViewModel() { Name = s.Name })
                .ToList();

            _satellites.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(satellites);
            });
        }
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
    public bool? IsAllSatellites { get; set; }
}