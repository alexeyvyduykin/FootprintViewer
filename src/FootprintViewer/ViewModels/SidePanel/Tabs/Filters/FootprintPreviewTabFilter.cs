using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SensorItemViewModel : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; } = string.Empty;

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class FootprintPreviewTabFilter : BaseFilterViewModel<FootprintPreviewViewModel>
    {
        private readonly Data.DataManager.IDataManager _dataManager;
        private IDictionary<string, Geometry>? _geometries;
        private readonly IProvider<(string, Geometry)> _footprintPreviewGeometryProvider;

        public FootprintPreviewTabFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<IProvider<(string, Geometry)>>();
            _dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

            Sensors = new ObservableCollection<SensorItemViewModel>();

            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;
            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

            Observable.StartAsync(CreateSensorList).Subscribe();
        }

        public override IObservable<Func<FootprintPreviewViewModel, bool>> FilterObservable =>
            this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation, s => s.IsFullCoverAOI, s => s.AOI)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this)
            .Select(CreatePredicate);

        private static Func<FootprintPreviewViewModel, bool> CreatePredicate(FootprintPreviewTabFilter filter)
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
                    if (filter._geometries != null && filter._geometries.ContainsKey(footprintPreview.Name))
                    {
                        var footprintPolygon = (Polygon)filter._geometries[footprintPreview.Name];
                        var aoiPolygon = (Polygon)filter.AOI;

                        isAoiCondition = aoiPolygon.Intersection(footprintPolygon, filter.IsFullCoverAOI);
                    }
                }

                if (isAoiCondition == true)
                {
                    if (filter.Sensors.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprintPreview.SatelliteName) == true)
                    {
                        if (footprintPreview.CloudCoverFull >= filter.Cloudiness)
                        {
                            if (footprintPreview.SunElevation >= filter.MinSunElevation && footprintPreview.SunElevation <= filter.MaxSunElevation)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            };
        }

        private async Task CreateSensorList()
        {
            var footprints = await _dataManager.GetDataAsync<FootprintPreview>(DbKeys.FootprintPreviews.ToString());

            var dicts = await _footprintPreviewGeometryProvider.GetNativeValuesAsync(null);

            var sortNames = footprints.Select(s => s.SatelliteName).Distinct().ToList();

            AddSensors(sortNames!);

            // TODO: duplicates
            _geometries = dicts.ToDictionary(s => s.Item1, s => s.Item2);
        }

        private void AddSensors(List<string> sensors)
        {
            var list = sensors.OrderBy(s => s).Select(s => new SensorItemViewModel() { Name = s }).ToList();

            Sensors = new ObservableCollection<SensorItemViewModel>(list);

            var databasesValid = Sensors
                .ToObservableChangeSet()
                .AutoRefresh(model => model.IsActive)
                .Subscribe(s =>
                {
                    var temp = Cloudiness;
                    Cloudiness = temp + 1;
                    Cloudiness = temp;
                });

            // HACK: call observable
            var temp = Cloudiness;
            Cloudiness = temp + 1;
            Cloudiness = temp;
        }

        public override bool Filtering(FootprintPreviewViewModel value)
        {
            bool isAoiCondition = false;

            if (AOI == null)
            {
                isAoiCondition = true;
            }
            else
            {
                if (_geometries != null && _geometries.ContainsKey(value.Name))
                {
                    var footprintPolygon = (Polygon)_geometries[value.Name];
                    var aoiPolygon = (Polygon)AOI;

                    isAoiCondition = aoiPolygon.Intersection(footprintPolygon, IsFullCoverAOI);
                }
            }

            if (isAoiCondition == true)
            {
                if (Sensors.Where(s => s.IsActive == true).Select(s => s.Name).Contains(value.SatelliteName) == true)
                {
                    if (value.CloudCoverFull >= Cloudiness)
                    {
                        if (value.SunElevation >= MinSunElevation && value.SunElevation <= MaxSunElevation)
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

        [Reactive]
        public ObservableCollection<SensorItemViewModel> Sensors { get; set; }

        [Reactive]
        public bool IsAllSensorActive { get; set; }

        [Reactive]
        public Geometry? AOI { get; set; }

        public override string[]? Names => throw new NotImplementedException();
    }
}
