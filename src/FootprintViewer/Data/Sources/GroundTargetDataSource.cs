using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundTargetDataSource : IGroundTargetDataSource
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public GroundTargetDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync()
        {
            FootprintViewerDbContext context = new FootprintViewerDbContext(_options);

            return await context.GroundTargets.ToListAsync();
        }
    }
}
