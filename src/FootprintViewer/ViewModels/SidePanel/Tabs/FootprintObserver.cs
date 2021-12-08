using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using FootprintViewer.Data;
using System.Threading;
using System.Linq;
using NetTopologySuite.Geometries;

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

        public FootprintObserver() { }

        public FootprintObserver(Map map)
        {
            FootprintInfos = new ObservableCollection<FootprintInfo>();

            _footrpintLayer = map.GetLayer<FootprintLayer>(LayerType.Footprint);

            Filter = new FootprintObserverFilter();

            PreviewMouseLeftButtonDownCommand = ReactiveCommand.Create(PreviewMouseLeftButtonDown);

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
                }         
            });

            Type = FootprintViewerContentType.Update;
        }

        public ReactiveCommand<Unit, Unit> PreviewMouseLeftButtonDownCommand { get; }

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
            }
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

            FootprintInfos = new ObservableCollection<FootprintInfo>(footprints.Select(s => new FootprintInfo(s)));
            
            Type = FootprintViewerContentType.Show;
        }

        [Reactive]
        public FootprintObserverFilter Filter { get; set; }

        [Reactive]
        public FootprintViewerContentType Type { get; set; }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }
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
