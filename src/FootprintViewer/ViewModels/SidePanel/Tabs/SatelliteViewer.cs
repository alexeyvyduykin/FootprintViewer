using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;

namespace FootprintViewer.ViewModels
{  
    public class SatelliteViewer : SidePanelTab
    {
        private readonly TrackLayer _trackLayer;
        private readonly SensorLayer _sensorLayer;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        { 
            var map = dependencyResolver.GetService<Map>();
            var dataSource = dependencyResolver.GetService<IDataSource>();

            Title = "Просмотр спутников";
            Name = "SatelliteViewer";
            SatelliteInfos = new ObservableCollection<SatelliteInfo>();

            if (map == null)
            {
                return;
            }

            _trackLayer = map.GetLayer<TrackLayer>(LayerType.Track);
            _sensorLayer = map.GetLayer<SensorLayer>(LayerType.Sensor);
          
            SatelliteInfos.CollectionChanged += items_CollectionChanged;

            this.WhenAnyValue(s => s.DataSource).Subscribe(_ => DataSourceChanged());

            DataSource = dataSource;
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
}
