using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class UserGeometryDataSource : IEditableDataSource<UserGeometry>
    {
        private readonly DbContextOptions<UserGeometryDbContext> _options;
        private readonly string? _tableName;

        public UserGeometryDataSource(DbContextOptions<UserGeometryDbContext> options, string tableName)
        {
            _options = options;
            _tableName = tableName;
        }

        public async Task AddAsync(UserGeometry value)
        {
            using var context = new UserGeometryDbContext(_tableName, _options);

            await context.UserGeometries.AddAsync(value);

            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(UserGeometry value)
        {
            using var context = new UserGeometryDbContext(_tableName, _options);

            context.UserGeometries.Remove(value);

            await context.SaveChangesAsync();
        }

        public async Task EditAsync(string key, UserGeometry value)
        {
            using var context = new UserGeometryDbContext(_tableName, _options);

            var userGeometry = await context.UserGeometries
                .Where(b => b.Name == key)
                .FirstOrDefaultAsync();

            if (userGeometry != null)
            {
                userGeometry.Geometry = value.Geometry;

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<UserGeometry>> GetNativeValuesAsync(IFilter<UserGeometry>? filter)
        {
            using var context = new UserGeometryDbContext(_tableName, _options);

            if (filter == null || filter.Names == null)
            {
                return await context.UserGeometries.ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<UserGeometry, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.UserGeometries
                  .Where(predicate).ToListAsync();
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<UserGeometry, T> converter)
        {
            using var context = new UserGeometryDbContext(_tableName, _options);

            if (filter == null || filter.Names == null)
            {
                return await context.UserGeometries.Select(s => converter(s)).ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<UserGeometry, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.UserGeometries
                  .Where(predicate)
                  .Select(s => converter(s)).ToListAsync();
        }
    }
}
