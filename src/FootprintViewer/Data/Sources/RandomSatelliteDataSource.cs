using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : ISatelliteDataSource
    {
        private IEnumerable<Satellite>? _satellites;
        private IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>>? _tracks;
        private IDictionary<string, Dictionary<int, List<List<Point>>>>? _leftStrips;
        private IDictionary<string, Dictionary<int, List<List<Point>>>>? _rightStrips;

        public IEnumerable<Satellite> GetSatellites() => 
            _satellites ??= SatelliteBuilder.Create();

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks() => 
            _tracks ??= TrackBuilder.Create(GetSatellites());

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips() => 
            _leftStrips ??= StripBuilder.CreateLeft(GetSatellites());

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips() => 
            _rightStrips ??= StripBuilder.CreateRight(GetSatellites());
    }
}
