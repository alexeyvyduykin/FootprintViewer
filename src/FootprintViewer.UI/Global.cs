using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Helpers;
using FootprintViewer.Models;
using System.Collections.Generic;
using System.IO;

namespace FootprintViewer.UI;

public class Global
{
    public Global(string dataDir, Config config)
    {
        DataDir = dataDir;

        Config = config;

        MapSnapshotDir = Path.Combine(DataDir, "Snapshots");

        Directory.CreateDirectory(MapSnapshotDir);
    }

    public string DataDir { get; }

    public string MapSnapshotDir { get; }

    public Config Config { get; }

    //public static IDataManager CreateDataManager()
    //{
    //    //     var connectionString = ConnectionString.Build("localhost", 5432, "FootprintViewerDatabase", "postgres", "user").ToString();

    //    //  var dbFactory = new DbFactory();

    //    // userGeometries
    //    //    var userGeometriesKey = DbKeys.UserGeometries.ToString();
    //    //    var userGeometriesSource = dbFactory.CreateSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

    //    // plannedSchedules
    //    //    var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
    //    //    var plannedSchedulesSource = dbFactory.CreateSource(DbKeys.PlannedSchedules, connectionString, "PlannedSchedules");

    //    // maps
    //    var mapsKey = DbKeys.Maps;

    //    string embeddedFilePath = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

    //    var mapSource = new FileSource(new[] { embeddedFilePath }, MapResource.Build);

    //    var sources = new Dictionary<string, IList<ISource>>()
    //    {
    //        //{ userGeometriesKey, new[] { userGeometriesSource } },

    //        { mapsKey.ToString(), new[] { mapSource } },

    //        //{ plannedSchedulesKey, new[] { plannedSchedulesSource } }
    //    };

    //    return new DataManager(sources);
    //}

    public static IList<(string, ISource)> CreateDemoSources()
    {
        string embeddedFilePath = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "PlannedSchedule.json");

        //var connectionString = ConnectionString.Build("localhost", 5432, "FootprintViewerDatabase", "postgres", "user").ToString();

        var dbFactory = new DbFactory();

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        //var userGeometriesSource = dbFactory.CreateSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = dbFactory.CreateSource(DbKeys.PlannedSchedules, embeddedFilePath);

        return new List<(string, ISource)>()
        {
            //(userGeometriesKey, userGeometriesSource),
            (plannedSchedulesKey, plannedSchedulesSource)
        };
    }

    public static IList<(string, ISource)> CreateSources(Func<PlannedScheduleDbContext> creator)
    {
        // TODO: remove user geometry as default source
        var connectionString = ConnectionString.Build("localhost", 5432, "FootprintViewerDatabase", "postgres", "user").ToString();

        var dbFactory = new DbFactory();

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = dbFactory.CreateSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = new DatabaseSource(creator);

        return new List<(string, ISource)>()
        {
            (userGeometriesKey, userGeometriesSource),
            (plannedSchedulesKey, plannedSchedulesSource)
        };
    }

    public static IList<(string, ISource)> CreateSources(string filePath)
    {
        // userGeometries
        //var userGeometriesKey = DbKeys.UserGeometries.ToString();
        //var userGeometriesSource = dbFactory.CreateSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = new JsonSource(filePath, path => JsonHelpers.DeserializeFromFile<PlannedScheduleResult>(path)!);

        return new List<(string, ISource)>()
        {
            //(userGeometriesKey, userGeometriesSource),
            (plannedSchedulesKey, plannedSchedulesSource)
        };
    }

    //public static void AddLastPlannedSchedule(Config config, IDataManager dataManager)
    //{
    //    switch (config.PlannedScheduleState)
    //    {
    //        case Models.PlannedScheduleState.None:
    //            break;
    //        case Models.PlannedScheduleState.Demo:
    //            {
    //                foreach (var (key, source) in CreateDemoSources())
    //                {
    //                    dataManager.RegisterSource(key, source);
    //                }
    //            }
    //            break;
    //        case Models.PlannedScheduleState.JsonFile:
    //            {
    //                var path = config.LastPlannedScheduleJsonFile;

    //                // TODO: verify path
    //                if (System.IO.Path.Exists(path) == true)
    //                {
    //                    foreach (var (key, source) in CreateSources(path))
    //                    {
    //                        dataManager.RegisterSource(key, source);
    //                    }
    //                }
    //            }
    //            break;
    //        case Models.PlannedScheduleState.Database:
    //            {
    //                var connection = config.LastPlannedScheduleConnection;

    //                // TODO: verify connection
    //                if (connection is { })
    //                {
    //                    foreach (var (key, source) in CreateSources(() => new(connection.TableName, connection.ConnectionString)))
    //                    {
    //                        dataManager.RegisterSource(key, source);
    //                    }
    //                }
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public static IList<(string key, ISource source)> GetSources(Config config)
    {
        switch (config.PlannedScheduleState)
        {
            case Models.PlannedScheduleState.None:
                break;
            case Models.PlannedScheduleState.Demo:
                return CreateDemoSources();
            case Models.PlannedScheduleState.JsonFile:
                {
                    var path = config.LastPlannedScheduleJsonFile;

                    // TODO: verify path
                    if (System.IO.Path.Exists(path) == true)
                    {
                        return CreateSources(path);
                    }
                }
                break;
            case Models.PlannedScheduleState.Database:
                {
                    var connection = config.LastPlannedScheduleConnection;

                    // TODO: verify connection
                    if (connection is { })
                    {
                        return CreateSources(() => new(connection.TableName, connection.ConnectionString));
                    }
                }
                break;
            default:
                break;
        }

        return new List<(string, ISource)>();
    }
}
