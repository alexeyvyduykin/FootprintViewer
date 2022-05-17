﻿using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundStationDataSource : IDataSource<GroundStationInfo>
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public GroundStationDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task<List<GroundStationInfo>> GetValuesAsync(IFilter<GroundStationInfo>? filter = null)
        {
            var context = new FootprintViewerDbContext(_options);

            return await context.GroundStations.Select(s => new GroundStationInfo(s)).ToListAsync();
        }
    }
}
