using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Sources;
using FootprintViewer.Factories;
using FootprintViewer.FileSystem;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent;

public class Global
{
    public Global(Config config)
    {
        Config = config;

        MapFactory = new MapFactory();

        // DataManager
        DataManager = CreateDataManager();

        foreach (var item in config.MapBackgroundFiles)
        {
            DataManager.RegisterSource(DbKeys.Maps.ToString(), new FileSource(DbKeys.Maps.ToString(), new[] { item }));
        }

        //var uri = new Uri("avares://FootprintViewer.Fluent/Assets/world.mbtiles");

        //DataManager.RegisterSource(DbKeys.Maps.ToString(), new FileSource(DbKeys.Maps.ToString(), new[] { uri.AbsolutePath }));

        // LanguageManager
        LanguageManager = new LanguageManager(config.AvailableLocales);

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

    public Config Config { get; }

    public MapFactory MapFactory { get; }

    public ILanguageManager? LanguageManager { get; private set; }

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
        var connectionString = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = new EditableDatabaseSource(userGeometriesKey, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = new DatabaseSource(plannedSchedulesKey, connectionString, "PlannedSchedules");

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory1 = Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        var paths1 = Directory.GetFiles(directory1, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths2 = Directory.GetFiles(directory2, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource1 = new FileSource(mapsKey, paths1);
        var mapSource2 = new FileSource(mapsKey, paths2);

        // footprintPreviews
        var footprintPreviewsKey = DbKeys.FootprintPreviews.ToString();
        var directory3 = Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory4 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        var paths3 = Directory.GetFiles(directory3, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths4 = Directory.GetFiles(directory4, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource3 = new FileSource(footprintPreviewsKey, paths3);
        var mapSource4 = new FileSource(footprintPreviewsKey, paths4);

        // footprintPreviewGeometries
        var footprintPreviewGeometriesKey = DbKeys.FootprintPreviewGeometries.ToString();
        var path5 = new SolutionFolder("data").GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;
        var mapSource5 = new FileSource(footprintPreviewGeometriesKey, new List<string>() { path5 });

        var sources = new Dictionary<string, IList<ISource>>()
        {
            //{ userGeometriesKey, new[] { userGeometriesSource } },
            { mapsKey, new[] { mapSource1, mapSource2 } },
            //{ footprintPreviewsKey, new[] { mapSource3, mapSource4 } },
            //{ footprintPreviewGeometriesKey, new[] { mapSource5 } },
            //{ plannedSchedulesKey, new[] { plannedSchedulesSource } }
        };

        return new DataManager(sources);
    }

    public static IDataManager CreateDemoDataManager()
    {
        var connectionString = DbHelper.ToConnectionString("localhost", 5432, "FootprintViewerDatabase", "postgres", "user");

        // userGeometries
        var userGeometriesKey = DbKeys.UserGeometries.ToString();
        var userGeometriesSource = new EditableDatabaseSource(userGeometriesKey, connectionString, "UserGeometries");

        // plannedSchedules
        var plannedSchedulesKey = DbKeys.PlannedSchedules.ToString();
        var plannedSchedulesSource = new DatabaseSource(plannedSchedulesKey, connectionString, "PlannedSchedules");

        // maps
        var mapsKey = DbKeys.Maps.ToString();
        var directory1 = Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
        var directory2 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

        var paths1 = Directory.GetFiles(directory1, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths2 = Directory.GetFiles(directory2, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource1 = new FileSource(mapsKey, paths1);
        var mapSource2 = new FileSource(mapsKey, paths2);

        // footprintPreviews
        var footprintPreviewsKey = DbKeys.FootprintPreviews.ToString();
        var directory3 = Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
        var directory4 = Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

        var paths3 = Directory.GetFiles(directory3, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var paths4 = Directory.GetFiles(directory4, "*.mbtiles").Select(Path.GetFullPath).ToList();
        var mapSource3 = new FileSource(footprintPreviewsKey, paths3);
        var mapSource4 = new FileSource(footprintPreviewsKey, paths4);

        // footprintPreviewGeometries
        var footprintPreviewGeometriesKey = DbKeys.FootprintPreviewGeometries.ToString();
        var path5 = new SolutionFolder("data").GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff") ?? string.Empty;
        var mapSource5 = new FileSource(footprintPreviewGeometriesKey, new List<string>() { path5 });

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { userGeometriesKey, new[] { userGeometriesSource } },
            { mapsKey, new[] { mapSource1, mapSource2 } },
            { footprintPreviewsKey, new[] { mapSource3, mapSource4 } },
            { footprintPreviewGeometriesKey, new[] { mapSource5 } },
            { plannedSchedulesKey, new[] { plannedSchedulesSource } }
        };

        return new DataManager(sources);
    }
}
