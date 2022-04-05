using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class GroundStationInfo : ReactiveObject
    {
        private readonly Coordinate _center;
        private readonly string _name;

        public GroundStationInfo(GroundStation groundStation)
        {
            _name = groundStation.Name!;
            _center = groundStation.Center!.Coordinate.Copy();
        }

        public string Name => _name;

        public Coordinate Center => _center;
    }
}
