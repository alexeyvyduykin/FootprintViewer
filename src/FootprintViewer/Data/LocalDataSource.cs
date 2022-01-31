using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data
{
    public class LocalDataSource : IDataSource
    {
        private readonly IEnumerable<Satellite> _satellites;
        private readonly IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> _tracks;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _leftStrips;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _rightStrips;               

        public LocalDataSource()
        {
            _satellites = SatelliteBuilder.Create();

            _tracks = TrackBuilder.Create(_satellites);

            (_leftStrips, _rightStrips) = StripBuilder.Create(_satellites);       
        }

        public IEnumerable<Satellite> Satellites => _satellites;

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks => _tracks;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> LeftStrips => _leftStrips;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> RightStrips => _rightStrips;
    }
}
