using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundStationDataSource : IDataSource<GroundStationInfo>
    {
        private readonly DbContextOptions<DbCustomContext> _options;
        private readonly string? _tableName;

        public GroundStationDataSource(IDatabaseSourceInfo databaseInfo)
        {
            _options = databaseInfo.BuildDbContextOptions<DbCustomContext>();
            _tableName = databaseInfo.Table;
        }

        public async Task<List<GroundStationInfo>> GetValuesAsync(IFilter<GroundStationInfo>? filter = null)
        {
            using var context = new GroundStationDbContext(_tableName, _options);

            return await context.GroundStations.Select(s => new GroundStationInfo(s)).ToListAsync();
        }
    }
}
