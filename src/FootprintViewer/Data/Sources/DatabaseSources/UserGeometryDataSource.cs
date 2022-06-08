using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class UserGeometryDataSource : IEditableDataSource<UserGeometryInfo>
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public UserGeometryDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task AddAsync(UserGeometryInfo value)
        {
            var context = new FootprintViewerDbContext(_options);

            await context.UserGeometries.AddAsync(value.Geometry);

            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(UserGeometryInfo value)
        {
            var context = new FootprintViewerDbContext(_options);

            context.UserGeometries.Remove(value.Geometry);

            await context.SaveChangesAsync();
        }

        public async Task EditAsync(string key, UserGeometryInfo value)
        {
            var context = new FootprintViewerDbContext(_options);

            var userGeometry = await context.UserGeometries
                .Where(b => b.Name == key)
                .FirstOrDefaultAsync();

            if (userGeometry != null)
            {
                userGeometry.Geometry = value.Geometry.Geometry;

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<UserGeometryInfo>> GetValuesAsync(IFilter<UserGeometryInfo>? filter)
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
