using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class UserGeometryDataSource : IUserGeometryDataSource
    {
        private readonly FootprintViewerDbContext _context;

        public UserGeometryDataSource(FootprintViewerDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserGeometry geometry)
        {
            await _context.UserGeometries.AddAsync(geometry);
            await _context.SaveChangesAsync();
        }

        public void Remove(UserGeometry geometry)
        {
            _context.UserGeometries.Remove(geometry);
            _context.SaveChanges();
        }

        public async Task<List<UserGeometry>> GetUserGeometriesAsync() => await _context.UserGeometries.ToListAsync();
    }
}
