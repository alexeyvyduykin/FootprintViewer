﻿using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Helpers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent;

public class Global
{
    public Global(string dataDir, Config config)
    {
        DataDir = dataDir;

        Config = config;

        MapFactory = new MapFactory();

        // DataManager
        DataManager = CreateDataManager();

        foreach (var item in config.MapBackgroundFiles)
        {
            DataManager.RegisterSource(DbKeys.Maps.ToString(), new FileSource(new[] { item }, MapResource.Build));
        }

        AddLastPlannedSchedule(Config, DataManager);

        // LanguageManager
        //LanguageManager = new LanguageManager(new[] { "en" }/*config.AvailableLocales*/);

        MapSnapshotDir = Path.Combine(DataDir, "Snapshots");

        Directory.CreateDirectory(MapSnapshotDir);

        LayerStyleManager = new LayerStyleManager();

        FeatureManager = MapFactory.CreateFeatureManager();

        // StateMachines
        MapState = new MapState();

        // Layer providers
        GroundTargetProvider = new GroundTargetProvider(DataManager, LayerStyleManager);
        TrackProvider = new TrackProvider(DataManager, LayerStyleManager);
        SensorProvider = new SensorProvider(DataManager, LayerStyleManager);
        GroundStationProvider = new GroundStationProvider(DataManager, LayerStyleManager);
        FootprintProvider = new FootprintProvider(DataManager, LayerStyleManager);
        UserGeometryProvider = new UserGeometryProvider(DataManager, LayerStyleManager);

        Dictionary<LayerType, IProvider> providers = new()
        {
            { LayerType.GroundStation, GroundStationProvider },
            { LayerType.GroundTarget, GroundTargetProvider  },
            { LayerType.Sensor,SensorProvider  },
            { LayerType.Track, TrackProvider },
            { LayerType.User, UserGeometryProvider },
            { LayerType.Footprint, FootprintProvider }
        };

        Map = MapFactory.CreateMap(LayerStyleManager, providers);

        MapNavigator = new MapNavigator((Map)Map);

        AreaOfInterest = new AreaOfInterest((Map)Map);
    }

    public string DataDir { get; }

    public string MapSnapshotDir { get; }

    public Config Config { get; }

    public MapFactory MapFactory { get; }

    public IDataManager? DataManager { get; private set; }

    public LayerStyleManager? LayerStyleManager { get; private set; }

    public FeatureManager? FeatureManager { get; private set; }

    public MapState? MapState { get; private set; }

    public GroundTargetProvider? GroundTargetProvider { get; private set; }

    public TrackProvider? TrackProvider { get; private set; }

    public SensorProvider? SensorProvider { get; private set; }

    public GroundStationProvider? GroundStationProvider { get; private set; }

    public FootprintProvider? FootprintProvider { get; private set; }

    public UserGeometryProvider? UserGeometryProvider { get; private set; }

    public Map? Map { get; private set; }

    public IMapNavigator? MapNavigator { get; private set; }

    public AreaOfInterest? AreaOfInterest { get; private set; }

    public async Task InitializeAsync()
    {

    }

    public async Task DisposeAsync()
    {

    }

    private IDataManager CreateDataManager()
    {
        var connectionString = ConnectionString.Build("localhost", 5432, "FootprintViewerDatabase", "postgres", "user").ToString();

        var dbFactory = new DbFactory();

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = dbFactory.CreateSource(DbKeys.UserGeometries, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = dbFactory.CreateSource(DbKeys.PlannedSchedules, connectionString, "PlannedSchedules");

        // maps
        var mapsKey = DbKeys.Maps;

        string embeddedFilePath = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mapSource = new FileSource(new[] { embeddedFilePath }, MapResource.Build);

        var sources = new Dictionary<string, IList<ISource>>()
        {
            //{ userGeometriesKey, new[] { userGeometriesSource } },

            { mapsKey.ToString(), new[] { mapSource } },

            //{ plannedSchedulesKey, new[] { plannedSchedulesSource } }
        };

        return new DataManager(sources);
    }

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

    private void AddLastPlannedSchedule(Config config, IDataManager dataManager)
    {
        switch (config.PlannedScheduleState)
        {
            case Models.PlannedScheduleState.None:
                break;
            case Models.PlannedScheduleState.Demo:
                {
                    foreach (var (key, source) in CreateDemoSources())
                    {
                        dataManager.RegisterSource(key, source);
                    }
                }
                break;
            case Models.PlannedScheduleState.JsonFile:
                {
                    var path = config.LastPlannedScheduleJsonFile;

                    // TODO: verify path
                    if (System.IO.Path.Exists(path) == true)
                    {
                        foreach (var (key, source) in CreateSources(path))
                        {
                            dataManager.RegisterSource(key, source);
                        }
                    }
                }
                break;
            case Models.PlannedScheduleState.Database:
                {
                    var connection = config.LastPlannedScheduleConnection;

                    // TODO: verify connection
                    if (connection is { })
                    {
                        foreach (var (key, source) in CreateSources(() => new(connection.TableName, connection.ConnectionString)))
                        {
                            dataManager.RegisterSource(key, source);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}