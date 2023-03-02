using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Databases;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseCreatorSample;

class Program
{
    static async Task Main(string[] _)
    {
        var connectionString1 = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");
        var connectionString2 = DbHelper.ToConnectionString("localhost", 5432, "PlannedScheduleDatabase", "postgres", "user");

        var satellites = await RandomModelBuilder.BuildRandomSatellitesAsync(5);
        var gss = await RandomModelBuilder.BuildRandomGroundStationsAsync(6);
        var footprints = await RandomModelBuilder.BuildRandomFootprintsAsync(satellites, 2000);
        var gts = await RandomModelBuilder.BuildRandomGroundTargetsAsync(footprints, 5000);

        var tasks = await RandomModelBuilder.BuildTasksAsync(gts);
        var observationTasks = await RandomModelBuilder.BuildObservationTaskResultsAsync(tasks, footprints);

        var plannedSchedule = new PlannedScheduleResult()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Tasks = tasks,
            PlannedSchedules = observationTasks
        };

        Console.WriteLine("Model load");

        using var db1 = new FootprintViewerDatabase(GetOptions<FootprintViewerDatabase>(connectionString1));
        using var db2 = new PlannedScheduleDatabase(GetOptions<PlannedScheduleDatabase>(connectionString2));

        var dbs = new DbContext[] { db1, db2 };

        CreateDb(dbs);

        db1.Satellites.AddRange(satellites);
        db1.GroundTargets.AddRange(gts);
        db1.Footprints.AddRange(footprints);
        db1.GroundStations.AddRange(gss);

        db2.Satellites.AddRange(satellites);
        db2.GroundTargets.AddRange(gts);
        db2.GroundStations.AddRange(gss);
        db2.PlannedSchedules.Add(plannedSchedule);

        SaveDb(dbs);

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
