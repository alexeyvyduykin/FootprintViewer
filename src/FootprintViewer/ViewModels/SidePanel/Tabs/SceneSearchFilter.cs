﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Binding;
using System.Reactive.Linq;
using DynamicData;
using Mapsui.Geometries;
using Splat;
using FootprintViewer.Data;

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

        public event EventHandler? Update;

        public SceneSearchFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            var footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<FootprintPreviewGeometryProvider>();

            _geometries = footprintPreviewGeometryProvider.GetFootprintPreviewGeometries();

            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;
            FromDate = DateTime.Today.AddDays(-1);
            ToDate = DateTime.Today.AddDays(1);

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

        public void AddSensors(List<string> sensors)
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
                if (_geometries.ContainsKey(footprint.Name))
                {
                    var footprintPolygon = (Polygon)_geometries[footprint.Name];
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

        //public IObservable<Func<Footprint, bool>> Observable => _observableFilter;

        private Func<FootprintPreview, bool> MakeFilter()
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
}
