using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class SatelliteDataManager : BaseDataManager<Satellite, IDatabaseSource>
    {
        protected override async Task<List<Satellite>> GetNativeValuesAsync(IDatabaseSource dataSource, IFilter<Satellite>? filter)
        {
            var options = extns2.BuildDbContextOptions<SatelliteDbContext>(dataSource);
            using var context = new SatelliteDbContext(dataSource.Table, options);

            return await context.Satellites.OrderBy(s => s.Name).ToListAsync();
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IDatabaseSource dataSource, IFilter<T>? filter, Func<Satellite, T> converter)
        {
            var options = extns2.BuildDbContextOptions<SatelliteDbContext>(dataSource);
            using var context = new SatelliteDbContext(dataSource.Table, options);

            return await context.Satellites.OrderBy(s => s.Name).Select(s => converter(s)).ToListAsync();
        }
    }
}
