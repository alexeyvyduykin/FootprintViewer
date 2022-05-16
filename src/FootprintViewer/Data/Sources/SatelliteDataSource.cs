using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<SatelliteInfo>> GetSatelliteInfosAsync()
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.Satellites.OrderBy(s => s.Name).Select(s => new SatelliteInfo(s)).ToListAsync();
        }
    }
}
