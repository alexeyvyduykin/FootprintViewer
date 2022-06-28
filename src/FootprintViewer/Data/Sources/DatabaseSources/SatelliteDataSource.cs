using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : IDataSource<Satellite>
    {
        private readonly DbContextOptions<SatelliteDbContext> _options;
        private readonly string? _tableName;

        public SatelliteDataSource(DbContextOptions<SatelliteDbContext> options, string tableName)
        {
            _options = options;

            _tableName = tableName;
        }

        public async Task<List<Satellite>> GetNativeValuesAsync(IFilter<Satellite>? filter)
        {
            using var context = new SatelliteDbContext(_tableName, _options);

            return await context.Satellites.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<Satellite, T> converter)
        {
            using var context = new SatelliteDbContext(_tableName, _options);

            return await context.Satellites.OrderBy(s => s.Name).Select(s => converter(s)).ToListAsync();
        }
    }
}
