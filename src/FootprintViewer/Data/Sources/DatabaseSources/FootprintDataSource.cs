using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintDataSource : IDataSource<FootprintInfo>
    {
        private readonly DbContextOptions<FootprintDbContext> _options;

        public FootprintDataSource(DbContextOptions<FootprintDbContext> options)
        {
            _options = options;
        }

        public async Task<List<FootprintInfo>> GetValuesAsync(IFilter<FootprintInfo>? filter = null)
        {
            var context = new FootprintDbContext(_options);

            if (filter == null)
            {
                return await context.Footprints.Select(s => new FootprintInfo(s)).ToListAsync();
            }

            var res = await context.Footprints.Select(s => new FootprintInfo(s)).ToListAsync();

            return await Task.Run(() => res.Where(s => filter.Filtering(s)).ToList());
        }
    }
}
