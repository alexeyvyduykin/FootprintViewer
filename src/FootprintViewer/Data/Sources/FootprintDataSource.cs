using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<List<Footprint>> GetFootprintsAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.Footprints.ToListAsync();
        }
    }
}
