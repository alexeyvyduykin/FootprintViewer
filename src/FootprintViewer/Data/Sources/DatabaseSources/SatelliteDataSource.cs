﻿using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class SatelliteDataSource : IDataSource<SatelliteInfo>
    {
        private readonly DbContextOptions<SatelliteDbContext> _options;
        private readonly string? _tableName;

        public SatelliteDataSource(string? tableName, DbContextOptions<SatelliteDbContext> options)
        {
            _tableName = tableName;
            _options = options;
        }

        public async Task<List<SatelliteInfo>> GetValuesAsync(IFilter<SatelliteInfo>? filter = null)
        {
            using var context = new SatelliteDbContext(_tableName, _options);

            return await context.Satellites.OrderBy(s => s.Name).Select(s => new SatelliteInfo(s)).ToListAsync();
        }
    }
}
