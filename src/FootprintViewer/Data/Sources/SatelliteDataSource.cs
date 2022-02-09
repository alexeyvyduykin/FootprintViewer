using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : ISatelliteDataSource
    {
        private readonly FootprintViewerDbContext _db;

        public SatelliteDataSource(FootprintViewerDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Satellite> GetSatellites() => _db.Satellites.OrderBy(s => s.Name);
    }
}
