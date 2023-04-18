using FootprintViewer.Data;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent;

public class Global
{
    public Global(Config config, AppMode mode)
    {
        Config = config;

        DataFactory = CreateDataFactory(mode);

        MapFactory = new MapFactory();

        // DataManager
        DataManager = DataFactory.CreateDataManager();

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

    public IDataFactory DataFactory { get; }

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

    private static IDataFactory CreateDataFactory(AppMode mode)
    {
        return mode switch
        {
            AppMode.Release => new ReleaseDataFactory(),
            AppMode.Demo => new DemoDataFactory(),
            AppMode.DevWork => new DevWorkDataFactory(),
            AppMode.DevHome => new DevHomeDataFactory(),
            _ => new ReleaseDataFactory()
        };
    }
}
