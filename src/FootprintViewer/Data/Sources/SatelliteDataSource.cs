using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : ISatelliteDataSource
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public SatelliteDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task<List<Satellite>> GetSatellitesAsync()
        {
            FootprintViewerDbContext context = new FootprintViewerDbContext(_options);

            return await context.Satellites.ToListAsync();
        }
    }
}
