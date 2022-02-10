using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class FootprintDataSource : IFootprintDataSource
    {
        private readonly FootprintViewerDbContext _context;

        public FootprintDataSource(FootprintViewerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Footprint>> GetFootprintsAsync() => await _context.Footprints.ToListAsync();
    }
}
