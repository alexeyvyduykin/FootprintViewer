using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class FootprintDataSource : IFootprintDataSource
    {
        private readonly FootprintViewerDbContext _db;

        public FootprintDataSource(FootprintViewerDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Footprint> GetFootprints() => _db.Footprints;
    }
}
