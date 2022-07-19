using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundStationDataSource : BaseDatabaseSource<DbCustomContext>, IDataSource<GroundStation>
    {
        public async Task<List<GroundStation>> GetNativeValuesAsync(IFilter<GroundStation>? filter)
        {
            using var context = new GroundStationDbContext(Table, Options);

            return await context.GroundStations.ToListAsync();
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<GroundStation, T> converter)
        {
            using var context = new GroundStationDbContext(Table, Options);

            return await context.GroundStations.Select(s => converter(s)).ToListAsync();
        }
    }
}
