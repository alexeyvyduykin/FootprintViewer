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
        var connectionString1 = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");
        var connectionString2 = DbHelper.ToConnectionString("localhost", 5432, "PlannedScheduleDatabase", "postgres", "user");

        var gts = await GroundTargetBuilder.CreateAsync(300);
        var satellites = await SatelliteBuilder.CreateAsync(5);
        var gss = await GroundStationBuilder.CreateDefaultAsync();

        // sats = 5, gts = 5000, res = 22880 [17 min]
        //var res = await ModelFactory.CreateTimeWindowsAsync(satellites, gts);

        //var footprints = await RandomModelBuilder.CreateFootprintsAsync(satellites, 2000);

        var plannedSchedule = await PlannedScheduleBuilder.CreateAsync(satellites, gts, gss);

        Console.WriteLine("Model load");

        using var db1 = new FootprintViewerDatabase(GetOptions<FootprintViewerDatabase>(connectionString1));
        using var db2 = new PlannedScheduleDatabase(GetOptions<PlannedScheduleDatabase>(connectionString2));

        var dbs = new DbContext[] { db1, db2 };

        var tempUserGeometry1 = db1.UserGeometries.ToList();
        var tempUserGeometry2 = db2.UserGeometries.ToList();

        CreateDb(dbs);

        db1.GroundTargets.AddRange(gts);
        //db1.Footprints.AddRange(footprints);
        db1.GroundStations.AddRange(gss);

        //db2.GroundTargets.AddRange(gts);
        //db2.GroundStations.AddRange(gss);
        db2.PlannedSchedules.Add(plannedSchedule);

        SaveDb(dbs);

        db1.UserGeometries.AddRange(tempUserGeometry1);
        db2.UserGeometries.AddRange(tempUserGeometry2);

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
