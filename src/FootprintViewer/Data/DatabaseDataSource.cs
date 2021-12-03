using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data
{
    public class DatabaseDataSource : IDataSource
    {
        private readonly FootprintViewerDbContext _db;
        private readonly IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> _tracks;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _leftStrips;
        private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _rightStrips;

        public DatabaseDataSource(FootprintViewerDbContext db)
        {
            _db = db;
            _tracks = TrackBuilder.Create(db.Satellites);
            (_leftStrips, _rightStrips) = StripBuilder.Create(db.Satellites);       
        }

        public IEnumerable<Footprint> Footprints => _db.Footprints; 
        
        public IEnumerable<Satellite> Satellites => _db.Satellites.OrderBy(s => s.Name);
        
        public IEnumerable<GroundTarget> Targets => _db.GroundTargets;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> LeftStrips => _leftStrips;

        public IDictionary<string, Dictionary<int, List<List<Point>>>> RightStrips => _rightStrips;

        public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks => _tracks;
    }
}
