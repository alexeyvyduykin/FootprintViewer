using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Databases;
using FootprintViewer.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseCreatorSample;

class Program
{
    static async Task Main(string[] _)
    {
        var connectionString = ConnectionString.Build("localhost", 5432, "FootprintViewerDatabase", "postgres", "user").ToString();

        var gts = await GroundTargetBuilder.CreateAsync(300);
        var satellites = await SatelliteBuilder.CreateAsync(5);
        var gss = await GroundStationBuilder.CreateDefaultAsync();

        // sats = 5, gts = 5000, res = 22880 [17 min]
        //var res = await ModelFactory.CreateTimeWindowsAsync(satellites, gts);

        //var footprints = await RandomModelBuilder.CreateFootprintsAsync(satellites, 2000);

        var plannedSchedule = await PlannedScheduleBuilder.CreateAsync(satellites, gts, gss);

        Console.WriteLine("Model load");

        using var db = new FootprintViewerDatabase(GetOptions<FootprintViewerDatabase>(connectionString));

        var tempUserGeometry = db.UserGeometries.ToList();

        CreateDb(new[] { db });

        db.PlannedSchedules.Add(plannedSchedule);

        SaveDb(new[] { db });

        db.UserGeometries.AddRange(tempUserGeometry);

        Console.WriteLine("Database build");
    }

    private static void CreateDb(IEnumerable<DbContext> dbs)
    {
        foreach (var item in dbs)
        {
            item.Database.EnsureDeleted();
            item.Database.EnsureCreated();
        }
    }

    private static void SaveDb(IEnumerable<DbContext> dbs)
    {
        foreach (var item in dbs)
        {
            item.SaveChanges();
        }
    }

    private static DbContextOptions<T> GetOptions<T>(string connectionString) where T : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<T>();
        var options = optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.UseNetTopologySuite();
        }).Options;

        return options;
    }
}
