﻿using DynamicData;
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

public class FootprintPreviewTabFilterViewModel : BaseFilterViewModel<FootprintPreviewViewModel>
{
    private readonly SourceList<SensorItemViewModel> _sensors = new();
    private readonly ReadOnlyObservableCollection<SensorItemViewModel> _items;
    private readonly SourceList<FootprintPreviewGeometry> _geometries = new();
    private readonly ReadOnlyObservableCollection<FootprintPreviewGeometry> _geometryItems;
    private readonly IDataManager _dataManager;
    private readonly IObservable<Func<FootprintPreviewViewModel, bool>> _filterObservable;

    public FootprintPreviewTabFilterViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        _sensors
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _geometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _geometryItems)
            .Subscribe();

        Cloudiness = 0.0;
        MinSunElevation = 0.0;
        MaxSunElevation = 90.0;
        IsFullCoverAOI = false;
        FromDate = DateTime.Today.AddDays(-1);
        ToDate = DateTime.Today.AddDays(1);

        _dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateImpl));

        var _activeChanged = _sensors.Connect().WhenValueChanged(p => p.IsActive);

        var observable1 = this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation, s => s.IsFullCoverAOI, s => s.AOI)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var observable2 = _activeChanged
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this);

        var merged = Observable.Merge(observable1, observable2);

        _filterObservable = merged.Select(CreatePredicate);

        Observable.StartAsync(UpdateImpl, RxApp.MainThreadScheduler).Subscribe();
    }

    public override IObservable<Func<FootprintPreviewViewModel, bool>> FilterObservable => _filterObservable;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<FootprintPreview>(DbKeys.FootprintPreviews.ToString());
        var geometries = await _dataManager.GetDataAsync<FootprintPreviewGeometry>(DbKeys.FootprintPreviewGeometries.ToString());

        var sensors = res
            .Where(s => string.IsNullOrEmpty(s.SatelliteName) == false)
            .Select(s => s.SatelliteName!)
            .Distinct()
            .OrderBy(s => s)
            .Select(s => new SensorItemViewModel() { Name = s })
            .ToList();

        _sensors.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(sensors);
        });

        _geometries.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(geometries);
        });
    }

    private static Func<FootprintPreviewViewModel, bool> CreatePredicate(FootprintPreviewTabFilterViewModel filter)
    {
        return footprintPreview =>
        {
            bool isAoiCondition = false;

            if (filter.AOI == null)
            {
                isAoiCondition = true;
            }
            else
            {
                var geometry = filter.Geometries
                    .Where(s => Equals(s.Name, footprintPreview.Name))
                    .Select(s => s.Geometry)
                    .FirstOrDefault();

                if (geometry != null)
                {
                    var footprintPolygon = (Polygon)geometry;
                    var aoiPolygon = (Polygon)filter.AOI;

                    isAoiCondition = aoiPolygon.Intersection(footprintPolygon, filter.IsFullCoverAOI);
                }
            }

            if (isAoiCondition == true)
            {
                var isFound = filter.Sensors.Where(s => s.IsActive == true)
                    .Select(s => s.Name)
                    .Contains(footprintPreview.SatelliteName);

                if (isFound == true)
                {
                    if (footprintPreview.CloudCoverFull >= filter.Cloudiness)
                    {
                        if (footprintPreview.SunElevation >= filter.MinSunElevation
                            && footprintPreview.SunElevation <= filter.MaxSunElevation)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        };
    }

    public override bool Filtering(FootprintPreviewViewModel footprintPreview)
    {
        bool isAoiCondition = false;

        if (AOI == null)
        {
            isAoiCondition = true;
        }
        else
        {
            var geometry = Geometries
                .Where(s => Equals(s.Name, footprintPreview.SatelliteName))
                .Select(s => s.Geometry)
                .FirstOrDefault();

            if (geometry != null)
            {
                var footprintPolygon = (Polygon)geometry;
                var aoiPolygon = (Polygon)AOI;

                isAoiCondition = aoiPolygon.Intersection(footprintPolygon, IsFullCoverAOI);
            }
        }

        if (isAoiCondition == true)
        {
            var isFound = Sensors
                .Where(s => s.IsActive == true)
                .Select(s => s.Name)
                .Contains(footprintPreview.SatelliteName);

            if (isFound == true)
            {
                if (footprintPreview.CloudCoverFull >= Cloudiness)
                {
                    if (footprintPreview.SunElevation >= MinSunElevation
                        && footprintPreview.SunElevation <= MaxSunElevation)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    [Reactive]
    public DateTime FromDate { get; set; }

    [Reactive]
    public DateTime ToDate { get; set; }

    [Reactive]
    public double Cloudiness { get; set; }

    [Reactive]
    public double MinSunElevation { get; set; }

    [Reactive]
    public double MaxSunElevation { get; set; }

    [Reactive]
    public bool IsFullCoverAOI { get; set; }

    public ReadOnlyObservableCollection<SensorItemViewModel> Sensors => _items;

    private ReadOnlyObservableCollection<FootprintPreviewGeometry> Geometries => _geometryItems;

    [Reactive]
    public Geometry? AOI { get; set; }

    public override string[]? Names => throw new NotImplementedException();
}
