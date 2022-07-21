using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class FootprintDataManager : BaseDataManager<Footprint, IDatabaseSource>
    {
        protected override async Task<List<Footprint>> GetNativeValuesAsync(IDatabaseSource dataSource, IFilter<Footprint>? filter)
        {
            var options = extns2.BuildDbContextOptions<FootprintDbContext>(dataSource);
            using var context = new FootprintDbContext(dataSource.Table, options);

            if (filter == null)
            {
                return await context.Footprints.ToListAsync();
            }

            var res = await context.Footprints.ToListAsync();

            return await Task.Run(() => res.Where(s => filter.Filtering(s)).ToList());
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IDatabaseSource dataSource, IFilter<T>? filter, Func<Footprint, T> converter)
        {
            var options = extns2.BuildDbContextOptions<FootprintDbContext>(dataSource);
            using var context = new FootprintDbContext(dataSource.Table, options);

            if (filter == null)
            {
                return await context.Footprints.Select(s => converter(s)).ToListAsync();
            }

            var res = await context.Footprints.Select(s => converter(s)).ToListAsync();

            return await Task.Run(() => res.Where(s => filter.Filtering(s)).ToList());
        }
    }
}
