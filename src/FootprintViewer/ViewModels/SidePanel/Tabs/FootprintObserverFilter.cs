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
    public class SatelliteItem : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; } = string.Empty;

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class FootprintObserverFilter : ReactiveObject
    {
        public FootprintObserverFilter()
        {
            IsLeftStrip = true;
            IsRightStrip = true;
            IsAllSatelliteActive = true;
        }
        
        [Reactive]
        public int FromNode { get; set; }

        [Reactive]
        public int ToNode { get; set; }

        [Reactive]
        public bool IsLeftStrip { get; set; }

        [Reactive]
        public bool IsRightStrip { get; set; }

        [Reactive]
        public bool IsAllSatelliteActive { get; set; }

        [Reactive]
        public ObservableCollection<SatelliteItem> Satellites { get; set; } = new ObservableCollection<SatelliteItem>();

        [Reactive]
        public IGeometry? AOI { get; set; }

        [Reactive]
        public bool IsOpen { get; set; }
    }

    public class FootprintObserverFilterDesigner : FootprintObserverFilter
    {
        public FootprintObserverFilterDesigner()
        {
            var sat1 = new SatelliteItem() { Name = "Satellite1" };
            var sat2 = new SatelliteItem() { Name = "Satellite2" };
            var sat3 = new SatelliteItem() { Name = "Satellite3" };
            var sat4 = new SatelliteItem() { Name = "Satellite4" };
            var sat5 = new SatelliteItem() { Name = "Satellite5" };

            FromNode = 1;
            ToNode = 15;

            Satellites = new ObservableCollection<SatelliteItem>(new[] { sat1, sat2, sat3, sat4, sat5 });
        }
    }
}
