using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class UserGeometryDataManager : BaseEditableDataManager<UserGeometry, IDatabaseSource>
    {
        protected override async Task AddAsync(IDatabaseSource source, UserGeometry value)
        {
            var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
            using var context = new UserGeometryDbContext(source.Table, options);

            await context.UserGeometries.AddAsync(value);

            await context.SaveChangesAsync();
        }

        protected override async Task RemoveAsync(IDatabaseSource source, UserGeometry value)
        {
            var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
            using var context = new UserGeometryDbContext(source.Table, options);

            context.UserGeometries.Remove(value);

            await context.SaveChangesAsync();
        }

        protected override async Task EditAsync(IDatabaseSource source, string key, UserGeometry value)
        {
            var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
            using var context = new UserGeometryDbContext(source.Table, options);

            var userGeometry = await context.UserGeometries
                .Where(b => b.Name == key)
                .FirstOrDefaultAsync();

            if (userGeometry != null)
            {
                userGeometry.Geometry = value.Geometry;

                await context.SaveChangesAsync();
            }
        }

        protected override async Task<List<UserGeometry>> GetNativeValuesAsync(IDatabaseSource source, IFilter<UserGeometry>? filter)
        {
            var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
            using var context = new UserGeometryDbContext(source.Table, options);

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

        protected override async Task<List<T>> GetValuesAsync<T>(IDatabaseSource source, IFilter<T>? filter, Func<UserGeometry, T> converter)
        {
            var options = extns2.BuildDbContextOptions<UserGeometryDbContext>(source);
            using var context = new UserGeometryDbContext(source.Table, options);

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
