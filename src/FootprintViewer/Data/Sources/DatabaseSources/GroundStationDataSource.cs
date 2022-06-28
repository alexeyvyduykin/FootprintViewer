using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundStationDataSource : IDataSource<GroundStation>
    {
        private readonly DbContextOptions<DbCustomContext> _options;
        private readonly string? _tableName;

        public GroundStationDataSource(DbContextOptions<DbCustomContext> options, string tableName)
        {
            _options = options;
            _tableName = tableName;
        }

        public async Task<List<GroundStation>> GetNativeValuesAsync(IFilter<GroundStation>? filter)
        {
            using var context = new GroundStationDbContext(_tableName, _options);

            return await context.GroundStations.ToListAsync();
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<GroundStation, T> converter)
        {
            using var context = new GroundStationDbContext(_tableName, _options);

            return await context.GroundStations.Select(s => converter(s)).ToListAsync();
        }
    }
}
