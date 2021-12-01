#nullable enable
using DatabaseCreatorSample.Data;
using DynamicData;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.UI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SatelliteGeometrySample.ViewModels
{
    public class SatelliteInfo : ReactiveObject
    {
        public SatelliteInfo(Satellite satellite)
        {
            Satellite = satellite;

            var count = satellite.ToPRDCTSatellite().Nodes().Count;

            MinNode = 1;
            MaxNode = count;

            CurrentNode = 1;

            //this.WhenAnyValue(s => s.CurrentNode, s => s.IsShow, s => s.IsTrack).Subscribe(_ =>
            //{
            //    test1();
            //  //  UpdateTrack?.Invoke(this, EventArgs.Empty);
            //});

            this.WhenAnyValue(s => s.CurrentNode, s => s.IsShow, s => s.IsLeftStrip, s => s.IsRightStrip).Subscribe(_ =>
            {              
                // UpdateStrips?.Invoke(this, EventArgs.Empty);               
            });
        }

        //public event EventHandler UpdateTrack;
        // public event EventHandler UpdateStrips;

        [Reactive]
        public Satellite Satellite { get; set; }

        [Reactive]
        public int MinNode { get; set; }

        [Reactive]
        public int MaxNode { get; set; }

        [Reactive]
        public int CurrentNode { get; set; }

        [Reactive]
        public bool IsShow { get; set; } = false;

        [Reactive]
        public bool IsTrack { get; set; } = true;

        [Reactive]
        public bool IsLeftStrip { get; set; } = true;

        [Reactive]
        public bool IsRightStrip { get; set; } = false;
    }

    public class FootprintInfo : ReactiveObject
    {
        public static FootprintInfo BuildFrom(Footprint footprint)
        {
            return new FootprintInfo()
            {
                Name = footprint.Name,
                SatelliteName = footprint.SatelliteName,
                TargetName = footprint.TargetName,
                Begin = footprint.Begin.ToString(),
                Duration = footprint.Duration,
                Node = footprint.Node,
                Direction = footprint.Direction == 0 ? "Left" : "Right",
            };
        }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public string? SatelliteName { get; set; }

        [Reactive]
        public string? TargetName { get; set; }

        [Reactive]
        public string? Begin { get; set; }

        [Reactive]
        public double Duration { get; set; }

        [Reactive]
        public int Node { get; set; }

        [Reactive]
        public string? Direction { get; set; }
    }

    public class DataViewModel : ReactiveObject
    {
        private readonly Map _map;
        private readonly ILayer _targetlayer;
        private readonly ILayer _sensorLayer;
        private readonly ILayer _trackLayer;
        private readonly FootprintLayer _footprintLayer;
        private readonly TargetProvider _targetProvider;
        private readonly TrackProvider _trackProvider;
        private readonly SensorProvider _sensorProvider;
        private readonly FootprintProvider _footprintProvider;

        public ObservableCollection<SatelliteInfo> SatelliteInfos { get; set; }

        public DataViewModel(Map map, IDataSource source)
        {
            _map = map;

            var list = source.Satellites.Select(s => new SatelliteInfo(s)).ToList();
            SatelliteInfos = new ObservableCollection<SatelliteInfo>();
            SatelliteInfos.CollectionChanged += items_CollectionChanged;
            SatelliteInfos.AddRange(list);

            _trackProvider = new TrackProvider(source);
            _sensorProvider = new SensorProvider(source);
            _footprintProvider = new FootprintProvider(source);
            _targetProvider = new TargetProvider(source);

            _targetlayer = CreateTargetLayer(_targetProvider);
            _sensorLayer = CreateSensorLayer(_sensorProvider);
            _trackLayer = CreateTrackLayer(_trackProvider);
            _footprintLayer = new FootprintLayer(_footprintProvider);

            _map.Layers.Add(_targetlayer);
            _map.Layers.Add(_sensorLayer);
            _map.Layers.Add(_trackLayer);
            _map.Layers.Add(_footprintLayer);

            SatelliteInfos[0].IsShow = true;
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
                    _trackProvider.Update(info);
                    _trackLayer.DataHasChanged();
                }

                if (e.PropertyName == nameof(SatelliteInfo.IsShow) ||
                    e.PropertyName == nameof(SatelliteInfo.CurrentNode) ||
                    e.PropertyName == nameof(SatelliteInfo.IsLeftStrip) ||
                    e.PropertyName == nameof(SatelliteInfo.IsRightStrip))
                {
                    _sensorProvider.Update(info);
                    _sensorLayer.DataHasChanged();
                }
            }
        }

        public void UpdateInfo(MapInfoEventArgs args)
        {
            if (args.MapInfo?.Feature != null)
            {
                var feature = args.MapInfo?.Feature;
                if (feature != null && feature.Fields.Contains("Name") == true)
                {
                    var name = (string)feature["Name"];

                    if (FootprintInfo != null && name.Equals(FootprintInfo.Name) == true)
                    {
                        return;
                    }

                    FootprintInfo = FootprintInfo.BuildFrom(_footprintProvider.GetFootprint(name));

                    _footprintProvider.SelectFeature(name);
                }
            }

            if (args.MapInfo != null)
            {
                MapInfo = args.MapInfo;
            }
        }

        [Reactive]
        public FootprintInfo FootprintInfo { get; set; }

        [Reactive]
        public MapInfo MapInfo { get; set; }

        private static ILayer CreateTrackLayer(IProvider provider)
        {
            return new MemoryLayer
            {
                Style = new VectorStyle
                {
                    Fill = null,
                    Line = new Pen(Color.Green, 1),
                },
                DataSource = provider,
            };
        }

        private static ILayer CreateSensorLayer(IProvider provider)
        {
            return new MemoryLayer
            {
                Style = new VectorStyle
                {
                    Fill = new Brush(Color.Opacity(Color.Blue, 0.25f)),
                    Line = null,
                    Outline = null,
                },
                DataSource = provider,
            };
        }

        private static ILayer CreateTargetLayer(IProvider provider)
        {
            return new MemoryLayer
            {
                Style = CreateTargetThemeStyle(),
                DataSource = provider,
            };
        }

        private static ThemeStyle CreateTargetThemeStyle()
        {
            return new ThemeStyle(f =>
            {
                if (f.Geometry is Point)
                {
                    return new SymbolStyle
                    {
                        Fill = new Brush(Color.Opacity(Color.Black, 0.3f)),
                        Outline = new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                        MaxVisible = 5000,
                    };
                }

                if (f.Geometry is LineString || f.Geometry is MultiLineString)
                {
                    return new VectorStyle
                    {
                        Fill = null,
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = 5000,
                    };
                }

                if (f.Geometry is Polygon || f.Geometry is MultiPolygon)
                {
                    return new VectorStyle
                    {
                        Fill = null,// new Brush(Color.Opacity(Color.Black, 0.3f)),
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = 5000,
                    };
                }

                throw new Exception();
            });
        }

    }
}
