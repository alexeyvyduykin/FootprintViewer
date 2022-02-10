﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using Mapsui.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SatelliteItem : ReactiveObject
    {
        public string? Name { get; set; }

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class FootprintObserverFilter : ReactiveObject
    {
        private readonly ReactiveCommand<FootprintObserverFilter, FootprintObserverFilter> update;

        public FootprintObserverFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            var provider = dependencyResolver.GetExistingService<SatelliteProvider>();

            Satellites = new ObservableCollection<SatelliteItem>();

            IsLeftStrip = true;
            IsRightStrip = true;
            IsAllSatelliteActive = true;
            FromNode = 1;
            ToNode = 15;

            update = ReactiveCommand.Create<FootprintObserverFilter, FootprintObserverFilter>(s => s);

            this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftStrip, s => s.IsRightStrip, s => s.Switcher)
                .Select(_ => this)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(f => update.Execute(f).Subscribe());

        //    CreateSatelliteList(provider);
        }

        public IObservable<FootprintObserverFilter> Update => update;

        public void ForceUpdate()
        {
            update.Execute().Subscribe();
        }

        private void CreateSatelliteList(SatelliteProvider provider)
        {
            var satelliteNames = provider.GetSatellites().Select(s => s.Name).ToList();

            satelliteNames?.Sort();

            var list = satelliteNames.Select(s => new SatelliteItem() { Name = s });

            Satellites = new ObservableCollection<SatelliteItem>(list);

            Satellites.ToObservableChangeSet()
                      .AutoRefresh(model => model.IsActive)
                      .Subscribe(s =>
                      {
                          Switcher = !Switcher;
                      });

            Switcher = !Switcher;
        }

        public bool Filtering(FootprintInfo footprint)
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
        private bool Switcher { get; set; }

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
        public ObservableCollection<SatelliteItem> Satellites { get; private set; }

        [Reactive]
        public IGeometry? AOI { get; set; }
    }
}
