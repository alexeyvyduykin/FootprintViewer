using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public class GroundStationInfo : ReactiveObject
    {
        private readonly Coordinate _center;
        private readonly string _name;
        private readonly double[] _angles;

        public GroundStationInfo(GroundStation groundStation)
        {
            _name = groundStation.Name!;
            _center = groundStation.Center.Coordinate.Copy();
            _angles = groundStation.Angles;
        }

        public string Name => _name;

        public Coordinate Center => _center;

        public double[] Angles => _angles;

        [Reactive]
        public bool IsShow { get; set; } = false;
    }
}
