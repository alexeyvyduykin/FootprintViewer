using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintDataSource : IFootprintDataSource
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public FootprintDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task<List<FootprintInfo>> GetFootprintInfosAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.Footprints.Select(s => new FootprintInfo(s)).ToListAsync();
        }
    }
}
