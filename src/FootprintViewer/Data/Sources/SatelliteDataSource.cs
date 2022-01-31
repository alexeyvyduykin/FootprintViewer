using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : ISatelliteDataSource
    {
        private readonly FootprintViewerDbContext _db;
        private IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>>? _tracks;
        private IDictionary<string, Dictionary<int, List<List<Point>>>>? _leftStrips;
        private IDictionary<string, Dictionary<int, List<List<Point>>>>? _rightStrips;

        public SatelliteDataSource(FootprintViewerDbContext db)
        {
            _db = db;                 
        }

        public IEnumerable<Satellite> GetSatellites() => _db.Satellites.OrderBy(s => s.Name);

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips() => 
            _leftStrips ??= StripBuilder.CreateLeft(GetSatellites());

        public IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips() => 
            _rightStrips ??= StripBuilder.CreateRight(GetSatellites());

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks() => 
            _tracks ??= TrackBuilder.Create(GetSatellites());
    }
}
