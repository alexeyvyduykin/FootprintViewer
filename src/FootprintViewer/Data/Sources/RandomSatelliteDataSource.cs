using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : ISatelliteDataSource
    {
        private readonly IEnumerable<Satellite> _satellites;
        private readonly IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> _tracks;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _leftStrips;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _rightStrips;

        public RandomSatelliteDataSource()
        {
            _satellites = SatelliteBuilder.Create();

            _tracks = TrackBuilder.Create(_satellites);

            (_leftStrips, _rightStrips) = StripBuilder.Create(_satellites);
        }

        public IEnumerable<Satellite> GetSatellites() => _satellites;

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks() => _tracks;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips() => _leftStrips;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips() => _rightStrips;
    }
}
