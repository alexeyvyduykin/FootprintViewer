using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class UserGeometryDataSource : IUserGeometryDataSource
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public UserGeometryDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task AddAsync(UserGeometry geometry)
        {
            var context = new FootprintViewerDbContext(_options);

            await context.UserGeometries.AddAsync(geometry);

            await context.SaveChangesAsync();
        }

        public void Remove(UserGeometry geometry)
        {
            var context = new FootprintViewerDbContext(_options);

            context.UserGeometries.Remove(geometry);

            context.SaveChanges();
        }

        public async Task<List<UserGeometry>> GetUserGeometriesAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.UserGeometries.ToListAsync();
        }
    }
}
