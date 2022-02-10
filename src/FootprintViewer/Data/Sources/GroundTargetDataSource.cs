using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundTargetDataSource : IGroundTargetDataSource
    {
        private readonly FootprintViewerDbContext _context;

        public GroundTargetDataSource(FootprintViewerDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync() => await _context.GroundTargets.ToListAsync();
    }
}
