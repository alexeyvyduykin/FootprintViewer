using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class Sensor : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; } = string.Empty;

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class SceneSearchFilter : ViewerListFilter<FootprintPreview>
    {
        private IDictionary<string, Geometry>? _geometries;
        private readonly FootprintPreviewGeometryProvider _footprintPreviewGeometryProvider;
        private readonly FootprintPreviewProvider _footprintPreviewProvider;

        public SceneSearchFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();
            _footprintPreviewProvider = dependencyResolver.GetExistingService<FootprintPreviewProvider>();

            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;
            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

            this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation, s => s.IsFullCoverAOI, s => s.AOI)
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(_ => Unit.Default)
                .InvokeCommand(Update);
        }

        protected async override Task InitImpl()
        {
            var footprints = await _footprintPreviewProvider.GetValuesAsync(null);
            var dicts = await _footprintPreviewGeometryProvider.GetValuesAsync();

            var sortNames = footprints.Select(s => s.SatelliteName).Distinct().ToList();

            AddSensors(sortNames!);

            // TODO: duplicates
            _geometries = dicts.ToDictionary(s => s.Item1, s => s.Item2);
        }

        private void AddSensors(List<string> sensors)
        {
            Sensors.Clear();

            foreach (var item in sensors.OrderBy(s => s))
            {
                Sensors.Add(new Sensor() { Name = item });
            }

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

        public override bool Filtering(FootprintPreview value)
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
        public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();

        [Reactive]
        public bool IsAllSensorActive { get; set; }

        [Reactive]
        public Geometry? AOI { get; set; }

        public override string[]? Names => throw new NotImplementedException();
    }
}
