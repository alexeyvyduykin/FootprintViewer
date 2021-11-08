using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using System.Reactive.Linq;
using System.Threading;
using DynamicData;
using Mapsui.Providers;
using Mapsui.Geometries.Utilities;
using Mapsui.Geometries;
//sing NetTopologySuite.Geometries;

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
        //private readonly IObservable<Func<Footprint, bool>> _observableFilter;


        public event EventHandler? Update;

        public SceneSearchFilter()
        {
            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;

            //this.WhenAnyValue(s => s.AOI).Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));

            this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation).
                Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));

            this.WhenAnyValue(s => s.IsFullCoverAOI).Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));

            //_observableFilter = 
            //    this.WhenAnyValue(s => s.Cloudiness, s => s.MinSunElevation, s => s.MaxSunElevation).
            //    Select(_ => MakeFilter());
        }

        public void ForceUpdate()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }

        public void AddSensors(IEnumerable<string> sensors)
        {
            Sensors.Clear();

            foreach (var item in sensors)
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

        public bool Filtering(Footprint footprint)
        {
            bool isAoiCondition = false; 

            if (AOI == null)
            {
                isAoiCondition = true;
            }
            else
            {
                var footprintPolygon = (Polygon)footprint.Geometry;
                var aoiPolygon = (Polygon)AOI;

                isAoiCondition = aoiPolygon.Intersection(footprintPolygon, IsFullCoverAOI);                       
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

        //public IObservable<Func<Footprint, bool>> Observable => _observableFilter;

        private Func<Footprint, bool> MakeFilter()
        {
            return footprint => 
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

                return false;
            };
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

    public class SceneSearchFilterDesigner : SceneSearchFilter
    {
        public SceneSearchFilterDesigner()
        {
            var sensor1 = new Sensor() { Name = "Satellite1 SNS-1" };
            var sensor2 = new Sensor() { Name = "Satellite1 SNS-2" };
            var sensor3 = new Sensor() { Name = "Satellite2 SNS-1" };
            var sensor4 = new Sensor() { Name = "Satellite3 SNS-1" };
            var sensor5 = new Sensor() { Name = "Satellite3 SNS-2" };

            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

            Sensors = new ObservableCollection<Sensor>(new[] { sensor1, sensor2, sensor3, sensor4, sensor5 });
        }
    }
}
