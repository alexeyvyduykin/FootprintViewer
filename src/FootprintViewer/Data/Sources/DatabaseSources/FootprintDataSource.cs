using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintDataSource : IDataSource<Footprint>
    {
        private readonly DbContextOptions<FootprintDbContext> _options;
        private readonly string? _tableName;

        public FootprintDataSource(DbContextOptions<FootprintDbContext> options, string tableName)
        {
            _options = options;
            _tableName = tableName;
        }

        public async Task<List<Footprint>> GetNativeValuesAsync(IFilter<Footprint>? filter)
        {
            using var context = new FootprintDbContext(_tableName, _options);

            if (filter == null)
            {
                return await context.Footprints.ToListAsync();
            }

            var res = await context.Footprints.ToListAsync();

            return await Task.Run(() => res.Where(s => filter.Filtering(s)).ToList());
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<Footprint, T> converter)
        {
            using var context = new FootprintDbContext(_tableName, _options);

            if (filter == null)
            {
                return await context.Footprints.Select(s => converter(s)).ToListAsync();
            }

            var res = await context.Footprints.Select(s => converter(s)).ToListAsync();

            return await Task.Run(() => res.Where(s => filter.Filtering(s)).ToList());
        }
    }
}
