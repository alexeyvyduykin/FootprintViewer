using DataSettingsSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class DatabaseSource<TContext> : BaseSource where TContext : DbCustomContext
    {
        public delegate TContext ContextBuilder(DbContextOptions<TContext> builder);
        public delegate DbContextOptions<TContext> OptionsBuilder(string connectionString);

        private readonly string _connectionString;
        private readonly ContextBuilder _contextBuilder;
        private readonly OptionsBuilder _optionsBuilder;

        public DatabaseSource(string connectionString, OptionsBuilder optionsBuilder, ContextBuilder contextBuilder)
        {
            _connectionString = connectionString;
            _contextBuilder = contextBuilder;
            _optionsBuilder = optionsBuilder;
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
              //  var options = extns2.BuildDbContextOptions<T>(_connectionString);

            var options = _optionsBuilder.Invoke(_connectionString);

            //using var context = new FootprintDbContext("Footprints", options);

            using var context = _contextBuilder.Invoke(options);

                //return await Task.Run(() => new List<Footprint>().Cast<object>().ToList());

                return await context.ToListAsync();     
        }
    }


    public class DatabaseSourceFootprint : BaseSource
    {
        private readonly string _connectionString;

        public DatabaseSourceFootprint(string connectionString)
        {
            _connectionString = connectionString;         
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            //var optionsBuilder = new DbContextOptionsBuilder<FootprintDbContext>();

            // var options = optionsBuilder.UseNpgsql(_connectionString).Options;

            await using var context = new FootprintDbContext("Footprints");//, options);
          //  await context.Database.EnsureDeletedAsync();
          //  await context.Database.EnsureCreatedAsync();


            return await context.Footprints.Cast<object>().ToListAsync();
        }
    }
    public class DatabaseSourceGroundTarget : BaseSource
    {
        private readonly string _connectionString;

        public DatabaseSourceGroundTarget(string connectionString)
        {
            _connectionString = connectionString;         
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
          //  var optionsBuilder = new DbContextOptionsBuilder<GroundTargetDbContext>();

         //   var options = optionsBuilder.UseNpgsql(_connectionString).Options;

         //   using var context = new GroundTargetDbContext("GroundTargets", options);



            await using var context = new GroundTargetDbContext("GroundTargets");
         //   await context.Database.EnsureDeletedAsync();
         //   await context.Database.EnsureCreatedAsync();


            return await context.GroundTargets.Cast<object>().ToListAsync();
        }
    }

    public class DatabaseSourceSatellite : BaseSource
    {
        private readonly string _connectionString;

        public DatabaseSourceSatellite(string connectionString)
        {
            _connectionString = connectionString;   
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
           // var optionsBuilder = new DbContextOptionsBuilder<SatelliteDbContext>();

          //  var options = optionsBuilder.UseNpgsql(_connectionString).Options;

          //  using var context = new SatelliteDbContext("Satellites", options);

            await using var context = new SatelliteDbContext("Satellites");
        //    await context.Database.EnsureDeletedAsync();
       //     await context.Database.EnsureCreatedAsync();


            return await context.Satellites.Cast<object>().ToListAsync();
        }
    }

    public class DatabaseSourceGroundStation : BaseSource
    {
        private readonly string _connectionString;

        public DatabaseSourceGroundStation(string connectionString)
        {
            _connectionString = connectionString;      
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
          //  var optionsBuilder = new DbContextOptionsBuilder<GroundStationDbContext>();

          //  var options = optionsBuilder.UseNpgsql(_connectionString).Options;

          //  using var context = new GroundStationDbContext("GroundStations", options);


            await using var context = new GroundStationDbContext("GroundStations");
         //   await context.Database.EnsureDeletedAsync();
         //   await context.Database.EnsureCreatedAsync();

            return await context.GroundStations.Cast<object>().ToListAsync();
        }
    }

    public class DatabaseSourceUserGeometry : BaseSource
    {
        private readonly string _connectionString;

        public DatabaseSourceUserGeometry(string connectionString)
        {
            _connectionString = connectionString;        
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
           // var optionsBuilder = new DbContextOptionsBuilder<UserGeometryDbContext>();

          //  var options = optionsBuilder.UseNpgsql(_connectionString).Options;

          //  using var context = new UserGeometryDbContext("UserGeometries", options);



            await using var context = new UserGeometryDbContext("UserGeometries");
         //   await context.Database.EnsureDeletedAsync();
         //   await context.Database.EnsureCreatedAsync();

            return await context.UserGeometries.Cast<object>().ToListAsync();
        }
    }

    public static class extns2
    {
        public static string ToConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public static DbContextOptions<T> BuildDbContextOptions<T>(string connectionString) where T : DbCustomContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            var options = optionsBuilder.UseNpgsql(connectionString).Options;

            return options;
        }
    }
}
