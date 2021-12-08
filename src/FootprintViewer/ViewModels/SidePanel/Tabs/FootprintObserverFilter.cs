using FootprintViewer.Models;
using Mapsui.Geometries;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FootprintViewer.Data;
using DynamicData.Binding;
using DynamicData;
using System.Linq;

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
        private int _counter = 0;

        public FootprintObserverFilter()
        {
            IsLeftStrip = true;
            IsRightStrip = true;
            IsAllSatelliteActive = true;
            FromNode = 1;
            ToNode = 15;

            this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftStrip, s => s.IsRightStrip, s => s.Counter).
                Subscribe(_ => Update?.Invoke(this, EventArgs.Empty));
        }

        public FootprintObserverFilter(IDataSource source) : this()
        {
            var satelliteNames = source.Satellites.Select(s => s.Name).ToList(); 
            satelliteNames.Sort();
            AddSatellites(satelliteNames);
        }

        public event EventHandler? Update;

        public void Click()
        {
            IsOpen = !IsOpen;
        }

        public void ForceUpdate()
        {
            Update?.Invoke(this, EventArgs.Empty);
        }

        private void AddSatellites(IEnumerable<string> satellites)
        {
            var list = new List<SatelliteItem>();

            foreach (var item in satellites)
            {
                list.Add(new SatelliteItem() { Name = item });
            }

            Satellites = new ObservableCollection<SatelliteItem>(list);

            var databasesValid = Satellites
                .ToObservableChangeSet()
                .AutoRefresh(model => model.IsActive)
                .Subscribe(s =>
                {
                    //var temp = FromNode;
                    //FromNode = temp + 1;
                    //FromNode = temp;

                    Counter = ++_counter;
                });

            // HACK: call observable
            //var temp = FromNode;
            //FromNode = temp + 1;
            //FromNode = temp;
            
            Counter = ++_counter;
        }

        public bool Filtering(Footprint footprint)
        {
            if (Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
            {
                if (footprint.Node >= FromNode && footprint.Node <= ToNode)
                {
                    if (footprint.Direction == SatelliteStripDirection.Left && IsLeftStrip == true)
                    {
                        return true;
                    }

                    if (footprint.Direction == SatelliteStripDirection.Right && IsRightStrip == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Reactive]
        private int Counter { get; set; }

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
