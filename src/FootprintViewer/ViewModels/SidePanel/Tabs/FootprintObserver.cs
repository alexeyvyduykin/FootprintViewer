﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Projection;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintInfo : ReactiveObject
    {
        private readonly Footprint _footprint;

        public FootprintInfo() { }

        public FootprintInfo(Footprint footprint)
        {
            _footprint = footprint;
            Name = footprint.Name;
            SatelliteName = footprint.SatelliteName;
            Center = footprint.Center.Coordinate.Copy();
            Begin = footprint.Begin;
            Duration = footprint.Duration;
            Node = footprint.Node;
            Direction = footprint.Direction;
        }

        public Footprint Footprint => _footprint;

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public bool IsShowInfo { get; set; } = false;

        [Reactive]
        public string? SatelliteName { get; set; }

        [Reactive]
        public Coordinate Center { get; set; } = new Coordinate();

        [Reactive]
        public DateTime Begin { get; set; }

        [Reactive]
        public double Duration { get; set; }

        [Reactive]
        public int Node { get; set; }

        [Reactive]
        public SatelliteStripDirection Direction { get; set; }
    }

    public enum FootprintViewerContentType
    {
        Show,
        Update
    }

    public class FootprintObserver : SidePanelTab
    {
        private readonly FootprintLayer _footrpintLayer;
        private readonly Map? _map;

        protected FootprintObserver() { }

        public FootprintObserver(Map map)
        {
            _map = map;

            _footrpintLayer = map.GetLayer<FootprintLayer>(LayerType.Footprint);

            FootprintInfos = new ObservableCollection<FootprintInfo>();
          
            PreviewMouseLeftButtonDownCommand = ReactiveCommand.Create(PreviewMouseLeftButtonDown);

            FilterClickCommand = ReactiveCommand.Create(FilterClick);

            this.WhenAnyValue(s => s.Type).Subscribe(type =>
            {
                if (type == FootprintViewerContentType.Update)
                {
                    FootprintsChanged();
                }
            });

            this.WhenAnyValue(s => s.IsActive).Subscribe(active =>
            {
                if (active == true)
                {
                    Type = FootprintViewerContentType.Update;
                }
            });

            this.WhenAnyValue(s => s.SelectedFootprintInfo).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

                    footprint.IsShowInfo = true;

                    if (ScrollToCenter == false)
                    {
                        if (string.IsNullOrEmpty(footprint.Name) == false)
                        {
                            _footrpintLayer.SelectFeature(footprint.Name);
                        }

                        SetMapFocusTo(footprint.Center);
                    }
                }
            });

            this.WhenAnyValue(s => s.PreviewMouseLeftButtonCommandChecker).Subscribe(_ => 
            {
                if (SelectedFootprintInfo != null && string.IsNullOrEmpty(SelectedFootprintInfo.Name) == false)
                {
                    if (SelectedFootprintInfo.IsShowInfo == true)
                    {
                        _footrpintLayer.SelectFeature(SelectedFootprintInfo.Name);                    
                    }
                    else
                    {
                        _footrpintLayer.UnselectFeature(SelectedFootprintInfo.Name);                        
                    }

                    _footrpintLayer.DataHasChanged();
                }
            });

            this.WhenAnyValue(s => s.Filter).Subscribe(_ => FilterChanged());

            Type = FootprintViewerContentType.Update;
        }

        private void FilterChanged()
        {
            if (Filter != null)
            {
                Filter.Update += (s, e) => Type = FootprintViewerContentType.Update;
            }
        }

        public ReactiveCommand<Unit, Unit> PreviewMouseLeftButtonDownCommand { get; }

        public ReactiveCommand<Unit, Unit> FilterClickCommand { get; }

        public void SelectFootprintInfo(string name)
        {
            var isSelect = _footrpintLayer.IsSelect(name);

            if (isSelect == true)
            {
                _footrpintLayer.UnselectFeature(name);

                FootprintInfos.ToList().ForEach(s => s.IsShowInfo = false);

                SelectedFootprintInfo = null;
            }
            else
            {         
                _footrpintLayer.SelectFeature(name);

                var item = FootprintInfos.Where(s => name.Equals(s.Name)).SingleOrDefault();

                if (item != null)
                {
                    ScrollCollectionToCenter(item);
                }
            }
        }

        private void ScrollCollectionToCenter(FootprintInfo item)
        {
            ScrollToCenter = true;

            SelectedFootprintInfo = item;

            ScrollToCenter = false;
        }

        private void SetMapFocusTo(Coordinate coordinate)
        {
            if (_map != null)
            {
                var p = SphericalMercator.FromLonLat(coordinate.X, coordinate.Y);

                _map.Initialized = false;

                _map.Home = (navigator) =>
                {
                    navigator.CenterOn(new Mapsui.Geometries.Point(p.X, p.Y));
                };

                // HACK: set Map.Initialized to false and add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                var layer = new Mapsui.Layers.Layer();
                _map.Layers.Add(layer);
                _map.Layers.Remove(layer);
            }
        }

        private void PreviewMouseLeftButtonDown()
        {
            if (SelectedFootprintInfo != null)
            {
                if (SelectedFootprintInfo.IsShowInfo == true)
                {
                    SelectedFootprintInfo.IsShowInfo = false;
                }
                else
                {
                    SelectedFootprintInfo.IsShowInfo = true;
                }

                PreviewMouseLeftButtonCommandChecker = !PreviewMouseLeftButtonCommandChecker;
            }
        }

        private void FilterClick()
        {
            Filter?.Click();
        }

        private static async Task<IList<Footprint>> LoadDataAsync(FootprintLayer layer)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return layer.GetFootprints().ToList();
            });
        }

        private async void FootprintsChanged()
        {
            var footprints = await LoadDataAsync(_footrpintLayer);

            if (footprints != null)
            {
                if (Filter == null)
                {
                    FootprintInfos = new ObservableCollection<FootprintInfo>(footprints.Select(s => new FootprintInfo(s)));
                }

                if (Filter != null)
                {
                    var list = new List<FootprintInfo>();

                    foreach (var item in footprints)
                    {
                        if (Filter.Filtering(item) == true)
                        {
                            list.Add(new FootprintInfo(item));
                        }
                    }

                    FootprintInfos = new ObservableCollection<FootprintInfo>(list);
                }
            }

            Type = FootprintViewerContentType.Show;
        }

        [Reactive]
        public FootprintObserverFilter? Filter { get; set; }

        [Reactive]
        public FootprintViewerContentType Type { get; set; }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }

        [Reactive]
        public bool ScrollToCenter { get; set; } = false;

        [Reactive]
        private bool PreviewMouseLeftButtonCommandChecker { get; set; } = false;
    }

    public class FootprintObserverDesigner : FootprintObserver
    {
        public FootprintObserverDesigner() : base()
        {
            Type = FootprintViewerContentType.Show;

            FootprintInfos = new ObservableCollection<FootprintInfo>()
            {
                new FootprintInfo(){ Name = "Footrpint0001", IsShowInfo = false },
                new FootprintInfo()
                {
                    Name = "Footrpint0002",
                    SatelliteName = "Satellite1",
                    IsShowInfo = true,
                    Center = new Coordinate(54.434545, -12.435454),
                    Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                    Duration = 35,
                    Node = 11,
                    Direction = SatelliteStripDirection.Left,
                },
                new FootprintInfo(){ Name = "Footrpint0003", IsShowInfo = false },
            };
        }
    }
}
