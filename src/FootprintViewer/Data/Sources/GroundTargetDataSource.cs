using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<List<GroundTarget>> GetGroundTargetsAsync(string[] names)
        {
            FootprintViewerDbContext context = new FootprintViewerDbContext(_options);

            return await context.GroundTargets.Where(s => names.Contains(s.Name)).ToListAsync();
        }

        public static Func<GroundTarget, bool> NameFilter(string[]? names = null)
        {
            return groundTarget => (names == null) || names.Contains(groundTarget.Name);
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(string[] names)
        {
            FootprintViewerDbContext context = new FootprintViewerDbContext(_options);

            return await context.GroundTargets.Where(s => names.Contains(s.Name)).Select(s => new GroundTargetInfo(s)).ToListAsync();
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosExAsync(Func<GroundTarget, bool> func)
        {
            FootprintViewerDbContext context = new FootprintViewerDbContext(_options);

            return await context.GroundTargets.Where(s => func(s)).Select(s => new GroundTargetInfo(s)).ToListAsync();
        }
    }
}
