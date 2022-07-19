using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : BaseDatabaseSource<SatelliteDbContext>, IDataSource<Satellite> 
    {
        public async Task<List<Satellite>> GetNativeValuesAsync(IFilter<Satellite>? filter)
        {
            using var context = new SatelliteDbContext(Table, Options);

            return await context.Satellites.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<Satellite, T> converter)
        {
            using var context = new SatelliteDbContext(Table, Options);

            return await context.Satellites.OrderBy(s => s.Name).Select(s => converter(s)).ToListAsync();
        }
    }
}
