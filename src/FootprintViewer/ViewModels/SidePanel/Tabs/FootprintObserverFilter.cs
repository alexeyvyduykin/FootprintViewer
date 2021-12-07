using FootprintViewer.Models;
using Mapsui.Geometries;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class FootprintObserverFilter : ReactiveObject
    {
        public FootprintObserverFilter()
        {
            Cloudiness = 0.0;
            MinSunElevation = 0.0;
            MaxSunElevation = 90.0;
            IsFullCoverAOI = false;
            IsAllSensorActive = true;
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

        [Reactive]
        public bool IsOpen { get; set; }
    }
}
