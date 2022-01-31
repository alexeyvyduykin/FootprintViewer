using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewer : SidePanelTab
    {
        private readonly TrackLayer? _trackLayer;
        private readonly SensorLayer? _sensorLayer;
        private readonly SatelliteProvider _provider;

        public SatelliteViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            var map = dependencyResolver.GetExistingService<Map>();
            _provider = dependencyResolver.GetExistingService<SatelliteProvider>();

            Title = "Просмотр спутников";
            Name = "SatelliteViewer";
            SatelliteInfos = new ObservableCollection<SatelliteInfo>();

            _trackLayer = map.GetLayer<TrackLayer>(LayerType.Track);
            _sensorLayer = map.GetLayer<SensorLayer>(LayerType.Sensor);

            SatelliteInfos.CollectionChanged += items_CollectionChanged;

            this.WhenAnyValue(s => s.IsActive).Subscribe(_ => DataSourceChanged());
        }

        private void DataSourceChanged()
        {
            var satellites = _provider.GetSatellites();

            SatelliteInfos.Clear();

            foreach (var item in satellites)
            {
                SatelliteInfos.Add(new SatelliteInfo(item));
            }
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged? item in e.OldItems)
                {
                    if (item != null)
                    {
                        item.PropertyChanged -= item_PropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged? item in e.NewItems)
                {
                    if (item != null)
                    {
                        item.PropertyChanged += item_PropertyChanged;
                    }
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
                    _trackLayer?.Update(info);
                }

                if (e.PropertyName == nameof(SatelliteInfo.IsShow) ||
                    e.PropertyName == nameof(SatelliteInfo.CurrentNode) ||
                    e.PropertyName == nameof(SatelliteInfo.IsLeftStrip) ||
                    e.PropertyName == nameof(SatelliteInfo.IsRightStrip))
                {
                    _sensorLayer?.Update(info);
                }
            }
        }

        [Reactive]
        public ObservableCollection<SatelliteInfo> SatelliteInfos { get; protected set; }
    }
}
