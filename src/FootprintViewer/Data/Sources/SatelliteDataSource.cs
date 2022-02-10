using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : ISatelliteDataSource
    {
        private readonly FootprintViewerDbContext _context;

        public SatelliteDataSource(FootprintViewerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Satellite>> GetSatellitesAsync() => await _context.Satellites.OrderBy(s => s.Name).ToListAsync();
    }
}
