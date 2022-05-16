using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry)
        {
            var context = new FootprintViewerDbContext(_options);

            var userGeometry = await context.UserGeometries
                .Where(b => b.Name == key)
                .FirstOrDefaultAsync();

            if (userGeometry != null)
            {
                userGeometry.Geometry = geometry;

                await context.SaveChangesAsync();
            }
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

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(IFilter<UserGeometryInfo>? filter)
        {
            var context = new FootprintViewerDbContext(_options);

            if (filter == null || filter.Names == null)
            {
                return await context.UserGeometries.Select(s => new UserGeometryInfo(s)).ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<UserGeometry, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.UserGeometries
                  .Where(predicate)
                  .Select(s => new UserGeometryInfo(s)).ToListAsync();
        }
    }
}
