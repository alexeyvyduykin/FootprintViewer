using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class UserGeometryDataSource : IUserGeometryDataSource
    {
        private readonly FootprintViewerDbContext _db;

        public UserGeometryDataSource(FootprintViewerDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(UserGeometry geometry)
        {
            await _db.UserGeometries.AddAsync(geometry);
            await _db.SaveChangesAsync();
        }

        public void Remove(UserGeometry geometry)
        {
            _db.UserGeometries.Remove(geometry);
            _db.SaveChanges();
        }

        public IEnumerable<UserGeometry> GetUserGeometries() => _db.UserGeometries;
    }
}
