﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public class SatelliteInfo : ReactiveObject
    {
        public SatelliteInfo() { }

        public SatelliteInfo(Satellite satellite)
        {
            Satellite = satellite;
            Name = satellite.Name;

            var count = satellite.ToPRDCTSatellite().Nodes().Count;
     
            MaxNode = count;

            ShowInfoClickCommand = ReactiveCommand.Create(ShowInfoClick);            
        }

        private void ShowInfoClick() => IsShowInfo = !IsShowInfo;

        public ReactiveCommand<Unit, Unit> ShowInfoClickCommand { get; }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public Satellite Satellite { get; set; }

        [Reactive]
        public int MinNode { get; set; } = 1;

        [Reactive]
        public int MaxNode { get; set; }

        [Reactive]
        public int CurrentNode { get; set; } = 1;

        [Reactive]
        public bool IsShow { get; set; } = false;

        [Reactive]
        public bool IsShowInfo { get; set; } = false;

        [Reactive]
        public bool IsTrack { get; set; } = true;

        [Reactive]
        public bool IsLeftStrip { get; set; } = true;

        [Reactive]
        public bool IsRightStrip { get; set; } = false;
    }

    public class SatelliteViewer : SidePanelTab
    {
        private readonly TrackLayer _trackLayer;
        private readonly SensorLayer _sensorLayer;

        public SatelliteViewer() { }

        public SatelliteViewer(Map map)
        {
            _trackLayer = map.GetLayer<TrackLayer>(LayerType.Track);
            _sensorLayer = map.GetLayer<SensorLayer>(LayerType.Sensor);

            SatelliteInfos = new ObservableCollection<SatelliteInfo>();
            SatelliteInfos.CollectionChanged += items_CollectionChanged;

            this.WhenAnyValue(s => s.DataSource).Subscribe(_ => DataSourceChanged());
        }

        private void DataSourceChanged()
        {
            if (DataSource != null)
            {
                var satellites = DataSource.Satellites;

                SatelliteInfos.Clear();

                foreach (var item in satellites)
                {
                    SatelliteInfos.Add(new SatelliteInfo(item));
                }

                //SatelliteInfos.AddRange(satellites.Select(s => new SatelliteInfo(s)));
            }
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is SatelliteInfo info)
            {
                if (e.PropertyName == nameof(SatelliteInfo.IsShow) ||
                    e.PropertyName == nameof(SatelliteInfo.CurrentNode) ||
                    e.PropertyName == nameof(SatelliteInfo.IsTrack))
                {
                    _trackLayer.Update(info);
                }

                if (e.PropertyName == nameof(SatelliteInfo.IsShow) ||
                    e.PropertyName == nameof(SatelliteInfo.CurrentNode) ||
                    e.PropertyName == nameof(SatelliteInfo.IsLeftStrip) ||
                    e.PropertyName == nameof(SatelliteInfo.IsRightStrip))
                {
                    _sensorLayer.Update(info);
                }
            }
        }

        [Reactive]
        public IDataSource? DataSource { get; set; }

        [Reactive]
        public ObservableCollection<SatelliteInfo> SatelliteInfos { get; protected set; }
    }

    public class SatelliteViewerDesigner : SatelliteViewer
    {
        public SatelliteViewerDesigner() : base()
        {
            var dt = new DateTime(2000, 6, 1, 12, 0, 0);
            var sat1 = new Satellite() { Semiaxis = 6945.03, Eccentricity = 0.0, InclinationDeg = 97.65, ArgumentOfPerigeeDeg = 0.0, LongitudeAscendingNodeDeg = 0.0, RightAscensionAscendingNodeDeg = 0.0, Period = 5760.0, Epoch = dt, InnerHalfAngleDeg = 32, OuterHalfAngleDeg = 48 };

            SatelliteInfos = new ObservableCollection<SatelliteInfo>()
            {
                new SatelliteInfo() { Name = "Satellite1", Satellite = sat1, IsShow = true,  IsShowInfo = false, MaxNode = 15 },
                new SatelliteInfo() { Name = "Satellite2", Satellite = sat1, IsShow = false, IsShowInfo = true, MaxNode = 15 },
                new SatelliteInfo() { Name = "Satellite3", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },
                new SatelliteInfo() { Name = "Satellite4", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },
                new SatelliteInfo() { Name = "Satellite5", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },
            };
        }
    }
}