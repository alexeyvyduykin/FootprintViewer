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

public sealed class FootprintPreviewTabFilterViewModel : AOIFilterViewModel<FootprintPreviewViewModel>
{
    private readonly SourceList<SensorItemViewModel> _sensors = new();
    private readonly ReadOnlyObservableCollection<SensorItemViewModel> _items;
    private readonly SourceList<FootprintPreviewGeometry> _geometries = new();
    private readonly ReadOnlyObservableCollection<FootprintPreviewGeometry> _geometryItems;
    private readonly IDataManager _dataManager;

    private const double CloudinessDefault = 0.0;
    private const double MinSunElevationDefault = 0.0;
    private const double MaxSunElevationDefault = 90.0;
    private static readonly DateTime FromDateDefault = DateTime.Today.AddDays(-1);
    private static readonly DateTime ToDateDefault = DateTime.Today.AddDays(1);

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
        FromDate = DateTime.Today.AddDays(-1);
        ToDate = DateTime.Today.AddDays(1);

        _dataManager.DataChanged
            .Where(s => new[] { DbKeys.FootprintPreviews.ToString(), DbKeys.FootprintPreviewGeometries.ToString() }.Any(key => s.Contains(key)))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateImpl));

        var _activeChanged = _sensors
            .Connect()
            .WhenValueChanged(p => p.IsActive);

        var observable1 = this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation)
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
            .Select(_ => Sensors.AllPropertyCheck(p => p.IsActive))
            .Subscribe(s => IsAllSensors = s);

        this.WhenAnyValue(s => s.IsAllSensors)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Select(s => (bool)s!)
            .Subscribe(value => Sensors.SetValue(s => s.IsActive = value));

        Observable.StartAsync(UpdateImpl, RxApp.MainThreadScheduler).Subscribe();
    }

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

    protected override void ResetImpl()
    {
        Cloudiness = CloudinessDefault;
        MinSunElevation = MinSunElevationDefault;
        MaxSunElevation = MaxSunElevationDefault;
        FromDate = FromDateDefault;
        ToDate = ToDateDefault;

        foreach (var item in Sensors)
        {
            item.IsActive = true;
        }
    }

    protected override bool IsDefaultImpl()
    {
        if (CloudinessDefault == Cloudiness
            && MinSunElevationDefault == MinSunElevation
            && MaxSunElevationDefault == MaxSunElevation
            && FromDateDefault == FromDate
            && ToDateDefault == ToDate)
        {
            return Sensors.All(s => s.IsActive == true);
        }

        return false;
    }

    protected override bool Filtering(FootprintPreviewViewModel footprintPreview)
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

        return false;
    }

    protected override bool AOIFiltering(FootprintPreviewViewModel footprintPreview)
    {
        if (IsAOIActive == true && AOI is Polygon aoiPolygon)
        {
            var geometry = Geometries
                .Where(s => Equals(s.Name, footprintPreview.Name))
                .Select(s => s.Geometry)
                .FirstOrDefault();

            if (geometry is Polygon polygon)
            {
                return aoiPolygon.Intersection(polygon, IsFullCoverAOI);
            }
        }

        return true;
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

    public ReadOnlyObservableCollection<SensorItemViewModel> Sensors => _items;

    private ReadOnlyObservableCollection<FootprintPreviewGeometry> Geometries => _geometryItems;

    [Reactive]
    public bool? IsAllSensors { get; set; }
}
