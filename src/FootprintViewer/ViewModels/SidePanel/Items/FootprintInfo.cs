using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace FootprintViewer.ViewModels
{
    public class FootprintInfo : ReactiveObject, IViewerItem
    {
        private readonly Footprint? _footprint;
        private readonly string _name;
        private readonly string _satelliteName;
        private readonly Coordinate _center;
        private readonly DateTime _begin;
        private readonly double _duration;
        private readonly int _node;
        private readonly SatelliteStripDirection _direction;

        public FootprintInfo(Footprint footprint)
        {
            _footprint = footprint;
            _name = footprint.Name!;
            _satelliteName = footprint.SatelliteName!;
            _center = footprint.Center!.Coordinate.Copy();
            _begin = footprint.Begin;
            _duration = footprint.Duration;
            _node = footprint.Node;
            _direction = footprint.Direction;
        }

        public Footprint? Footprint => _footprint;

        public string Name => _name;

        public string SatelliteName => _satelliteName;

        public Coordinate Center => _center;

        public DateTime Begin => _begin;

        public double Duration => _duration;

        public int Node => _node;

        public SatelliteStripDirection Direction => _direction;

        [Reactive]
        public bool IsShowInfo { get; set; } = false;
    }

}
