using System.Collections.Generic;

namespace FootprintViewer.Data.Sources
{
    public class GroundTargetDataSource : IGroundTargetDataSource
    {
        private readonly FootprintViewerDbContext _db;

        public GroundTargetDataSource(FootprintViewerDbContext db)
        {
            _db = db;
        }

        public IEnumerable<GroundTarget> GetGroundTargets() => _db.GroundTargets;        
    }
}
