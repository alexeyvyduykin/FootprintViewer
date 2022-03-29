using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using Mapsui.Geometries;
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

    public class SceneSearchFilter : ReactiveObject
    {
        private readonly IDictionary<string, IGeometry> _geometries;

        public SceneSearchFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            var footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();
            var footprintPreviewProvider = dependencyResolver.GetExistingService<FootprintPreviewProvider>();

            _geometries = footprintPreviewGeometryProvider.GetFootprintPreviewGeometries();

            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;
            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

            Update = ReactiveCommand.Create(() => this);

            this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation, s => s.IsFullCoverAOI, s => s.AOI)
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(_ => Unit.Default)
                .InvokeCommand(Update);

            Task.Run(async () => await CreateSensorList(footprintPreviewProvider));
        }

        public ReactiveCommand<Unit, SceneSearchFilter> Update { get; }

        private async Task CreateSensorList(FootprintPreviewProvider provider)
        {
            var footprints = await Task.Run(() => provider.GetFootprintPreviews());

            var sortNames = footprints.Select(s => s.SatelliteName).Distinct().ToList();

            AddSensors(sortNames!);
        }

        private void AddSensors(List<string> sensors)
        {
            //sensors.Sort();

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

        public bool Filtering(FootprintPreview footprint)
        {
            bool isAoiCondition = false;

            if (AOI == null)
            {
                isAoiCondition = true;
            }
            else
            {
                if (_geometries.ContainsKey(footprint.Name!))
                {
                    var footprintPolygon = (Polygon)_geometries[footprint.Name!];
                    var aoiPolygon = (Polygon)AOI;

                    isAoiCondition = aoiPolygon.Intersection(footprintPolygon, IsFullCoverAOI);
                }
            }

            if (isAoiCondition == true)
            {
                if (Sensors.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
                {
                    if (footprint.CloudCoverFull >= Cloudiness)
                    {
                        if (footprint.SunElevation >= MinSunElevation && footprint.SunElevation <= MaxSunElevation)
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
        public IGeometry? AOI { get; set; }
    }
}
