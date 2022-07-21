using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class GroundStationDataManager : BaseDataManager<GroundStation, IDatabaseSource>
    {
        protected override async Task<List<GroundStation>> GetNativeValuesAsync(IDatabaseSource dataSource, IFilter<GroundStation>? filter)
        {
            var options = extns2.BuildDbContextOptions<DbCustomContext>(dataSource);
            using var context = new GroundStationDbContext(dataSource.Table, options);

            return await context.GroundStations.ToListAsync();
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IDatabaseSource dataSource, IFilter<T>? filter, Func<GroundStation, T> converter)
        {
            var options = extns2.BuildDbContextOptions<DbCustomContext>(dataSource);
            using var context = new GroundStationDbContext(dataSource.Table, options);

            return await context.GroundStations.Select(s => converter(s)).ToListAsync();
        }
    }
}
