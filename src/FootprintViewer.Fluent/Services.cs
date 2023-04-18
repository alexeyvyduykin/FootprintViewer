using FootprintViewer.AppStates;
using FootprintViewer.Data;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;

namespace FootprintViewer.Fluent;

public static class Services
{
    public static IDataFactory DataFactory { get; private set; } = null!;

    public static MapFactory MapFactory { get; private set; } = null!;

    public static ILanguageManager LanguageManager { get; private set; } = null!;

    public static IDataManager DataManager { get; private set; } = null!;

    public static LayerStyleManager LayerStyleManager { get; private set; } = null!;

    public static FeatureManager FeatureManager { get; private set; } = null!;

    public static MapState MapState { get; private set; } = null!;

    public static GroundTargetProvider GroundTargetProvider { get; private set; } = null!;

    public static TrackProvider TrackProvider { get; private set; } = null!;

    public static SensorProvider SensorProvider { get; private set; } = null!;

    public static GroundStationProvider GroundStationProvider { get; private set; } = null!;

    public static FootprintProvider FootprintProvider { get; private set; } = null!;

    public static UserGeometryProvider UserGeometryProvider { get; private set; } = null!;

    public static Map Map { get; private set; } = null!;

    public static MainState? MainState { get; set; }

    public static IMapNavigator MapNavigator { get; private set; } = null!;

    public static AreaOfInterest AreaOfInterest { get; private set; } = null!;

    public static void Initialize(Global global)
    {
        DataFactory = global.DataFactory;

        MapFactory = global.MapFactory;

        LanguageManager = global.LanguageManager!;

        DataManager = global.DataManager!;

        LayerStyleManager = global.LayerStyleManager!;

        FeatureManager = global.FeatureManager!;

        MapState = global.MapState!;

        GroundTargetProvider = global.GroundTargetProvider!;

        TrackProvider = global.TrackProvider!;

        SensorProvider = global.SensorProvider!;

        GroundStationProvider = global.GroundStationProvider!;

        FootprintProvider = global.FootprintProvider!;

        UserGeometryProvider = global.UserGeometryProvider!;

        Map = global.Map!;

        MapNavigator = global.MapNavigator!;

        AreaOfInterest = global.AreaOfInterest!;
    }
}
