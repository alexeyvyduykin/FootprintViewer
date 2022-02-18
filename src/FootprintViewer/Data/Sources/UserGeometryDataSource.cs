using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task RemoveAsync(UserGeometry geometry)
        {
            var context = new FootprintViewerDbContext(_options);

            context.UserGeometries.Remove(geometry);

            await context.SaveChangesAsync();
        }

        public async Task<List<UserGeometry>> GetUserGeometriesAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.UserGeometries.ToListAsync();
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.UserGeometries.Select(s => new UserGeometryInfo(s)).ToListAsync();
        }
    }
}
