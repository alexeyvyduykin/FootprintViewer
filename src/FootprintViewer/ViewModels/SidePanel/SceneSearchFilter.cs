using HarfBuzzSharp;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
        public SceneSearchFilter()
        {

        }

        [Reactive]
        public DateTime FromDate { get; set; }

        [Reactive]
        public DateTime ToDate { get; set; }

        [Reactive]
        public double Cloudiness { get; set; } = 0.0;

        [Reactive]
        public double MinSunElevation { get; set; } = 0.0;

        [Reactive]
        public double MaxSunElevation { get; set; } = 90.0;

        [Reactive]
        public bool IsFullCoverAOI { get; set; } = false;

        [Reactive]
        public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>();

        [Reactive]
        public bool IsAllSensorActive { get; set; } = true;
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
