using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using FootprintViewer.ViewModels.ToolBar;
using Mapsui;
using Mapsui.Layers;
using NetTopologySuite.Geometries;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer;

internal sealed class DesignDataDependencyResolver : IReadonlyDependencyResolver
{
    private Map? _map;
    private IMapNavigator? _mapNavigator;
    private AreaOfInterest? _areaOfInterest;
    private ProjectFactory? _projectFactory;
    private GroundTargetProvider? _groundTargetProvider;
    private TrackProvider? _trackProvider;
    private SensorProvider? _sensorProvider;
    private GroundStationProvider? _groundStationProvider;
    private FootprintProvider? _footprintProvider;
    private UserGeometryProvider? _userGeometryProvider;
    private SatelliteTabViewModel? _satelliteTab;
    private FootprintTabViewModel? _footprintTab;
    private PlannedScheduleTabViewModel? _plannedScheduleTab;
    private GroundTargetTabViewModel? _groundTargetTab;
    private GroundStationTabViewModel? _groundStationTab;
    private UserGeometryTabViewModel? _userGeometryTab;
    private FootprintPreviewTabViewModel? _footprintPreviewTab;
    private MainViewModel? _mainViewModel;
    private SidePanelViewModel? _sidePanel;
    private ToolBarViewModel? _toolBar;
    private IDataManager? _dataManager;
    private ILanguageManager? _languageManager;
    private FeatureManager? _featureManager;
    private LayerStyleManager? _layerStyleManager;
    private MapState? _mapState;

    public object? GetService(Type? serviceType, string? contract = null)
    {
        if (serviceType == typeof(ProjectFactory))
        {
            return _projectFactory ??= new ProjectFactory(this);
        }
        else if (serviceType == typeof(IMap))
        {
            return _map ??= CreateMap();
        }
        else if (serviceType == typeof(LayerStyleManager))
        {
            return _layerStyleManager ??= new LayerStyleManager();
        }
        else if (serviceType == typeof(AreaOfInterest))
        {
            return _areaOfInterest ??= new AreaOfInterest((Map)GetService(typeof(IMap))!);
        }
        else if (serviceType == typeof(IMapNavigator))
        {
            return _mapNavigator ??= new MapNavigator((Map)GetService(typeof(IMap))!);
        }
        else if (serviceType == typeof(FeatureManager))
        {
            return _featureManager ??= new FeatureManager();
        }
        else if (serviceType == typeof(GroundTargetProvider))
        {
            return _groundTargetProvider ??= new GroundTargetProvider(this);
        }
        else if (serviceType == typeof(TrackProvider))
        {
            return _trackProvider ??= new TrackProvider(this);
        }
        else if (serviceType == typeof(SensorProvider))
        {
            return _sensorProvider ??= new SensorProvider(this);
        }
        else if (serviceType == typeof(GroundStationProvider))
        {
            return _groundStationProvider ??= new GroundStationProvider(this);
        }
        else if (serviceType == typeof(FootprintProvider))
        {
            return _footprintProvider ??= new FootprintProvider(this);
        }
        else if (serviceType == typeof(UserGeometryProvider))
        {
            return _userGeometryProvider ??= new UserGeometryProvider(this);
        }
        else if (serviceType == typeof(SatelliteTabViewModel))
        {
            return _satelliteTab ??= new SatelliteTabViewModel(this);
        }
        else if (serviceType == typeof(GroundStationTabViewModel))
        {
            return _groundStationTab ??= new GroundStationTabViewModel(this);
        }
        else if (serviceType == typeof(FootprintTabViewModel))
        {
            return _footprintTab ??= new FootprintTabViewModel(this);
        }
        else if (serviceType == typeof(PlannedScheduleTabViewModel))
        {
            return _plannedScheduleTab ??= new PlannedScheduleTabViewModel(this);
        }
        else if (serviceType == typeof(GroundTargetTabViewModel))
        {
            return _groundTargetTab ??= new GroundTargetTabViewModel(this);
        }
        else if (serviceType == typeof(FootprintPreviewTabViewModel))
        {
            return _footprintPreviewTab ??= new FootprintPreviewTabViewModel(this);
        }
        else if (serviceType == typeof(UserGeometryTabViewModel))
        {
            return _userGeometryTab ??= new UserGeometryTabViewModel(this);
        }
        else if (serviceType == typeof(ToolBarViewModel))
        {
            return _toolBar ??= new ToolBarViewModel(this);
        }
        else if (serviceType == typeof(SidePanelViewModel))
        {
            return _sidePanel ??= new SidePanelViewModel();
        }
        else if (serviceType == typeof(MainViewModel))
        {
            return _mainViewModel ??= new MainViewModel(this);
        }
        else if (serviceType == typeof(MapState))
        {
            return _mapState ??= new MapState();
        }
        else if (serviceType == typeof(IDataManager))
        {
            return _dataManager ??= CreateDataManager();
        }
        else if (serviceType == typeof(ILanguageManager))
        {
            return _languageManager ??= new LanguageManager(new LanguagesConfiguration() { AvailableLocales = new[] { "en", "ru" } });
        }
        throw new Exception();
    }

    public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
    {
        throw new Exception();
    }

    private static Map CreateMap()
    {
        var map = new Map();
        map.AddLayer(new Layer(), LayerType.WorldMap);
        map.AddLayer(new Layer(), LayerType.Footprint);
        map.AddLayer(new Layer(), LayerType.GroundTarget);
        map.AddLayer(new Layer(), LayerType.GroundStation);
        map.AddLayer(new Layer(), LayerType.Track);
        map.AddLayer(new Layer(), LayerType.Sensor);
        map.AddLayer(new Layer(), LayerType.User);
        return map;
    }

    private static DataManager CreateDataManager()
    {
        var source1 = new LocalSource<Footprint>(BuildFootprints);
        var source2 = new LocalSource<GroundTarget>(BuildGroundTargets);
        var source3 = new LocalSource<Satellite>(BuildSatellites);
        var source4 = new LocalSource<GroundStation>(BuildGroundStations);
        var source5 = new LocalSource<UserGeometry>(BuildUserGeometries);
        var source6 = new LocalSource<MapResource>(BuildMapResources);
        var source7 = new LocalSource<FootprintPreview>(BuildFootprintPreviews);
        var source8 = new LocalSource<FootprintPreviewGeometry>(BuildFootprintPreviewGeometries);
        var source9 = new LocalSource<PlannedScheduleResult>(BuildPlannedSchedule);

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { DbKeys.Footprints.ToString(), new[] { source1 } },
            { DbKeys.GroundTargets.ToString(), new[] { source2 } },
            { DbKeys.GroundStations.ToString(), new[] { source4 } },
            { DbKeys.UserGeometries.ToString(), new[] { source5 } },
            { DbKeys.Maps.ToString(), new[] { source6 } },
            { DbKeys.FootprintPreviews.ToString(), new[] { source7 } },
            { DbKeys.FootprintPreviewGeometries.ToString(), new[] { source8 } },
            { DbKeys.PlannedSchedules.ToString(), new[] { source9 } }
        };

        return new DataManager(sources);
    }

    private static List<T> Build<T>(int count, Func<T> func)
    {
        var tasks = new int[count]
            .Select(_ => Task<T>.Factory.StartNew(() => func()))
            .ToArray();

        Task.WaitAll(tasks);

        return tasks.Select(s => s.Result).ToList();
    }

    private static List<Satellite> BuildSatellites() => Build(5, SatelliteBuilder.CreateRandom);

    private static List<Footprint> BuildFootprints() => Build(10, FootprintBuilder.CreateRandom);

    private static List<PlannedScheduleResult> BuildPlannedSchedule() => Build(1, PlannedScheduleBuilder.CreateRandom);

    private static List<GroundTarget> BuildGroundTargets() => Build(10, GroundTargetBuilder.CreateRandom);

    private static List<UserGeometry> BuildUserGeometries() => Build(10, UserGeometryBuilder.CreateRandom);

    private static List<GroundStation> BuildGroundStations() =>
        new()
        {
                new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
        };

    private static List<MapResource> BuildMapResources() =>
        new()
        {
                new MapResource("WorldMapDefault", ""),
                new MapResource("OAM-World-1-8-min-J70", ""),
                new MapResource("OAM-World-1-10-J70", "")
        };

    private static List<FootprintPreview> BuildFootprintPreviews() => Build(8, FootprintPreviewBuilder.CreateRandom);

    private static List<FootprintPreviewGeometry> BuildFootprintPreviewGeometries() =>
        new()
        {
                new FootprintPreviewGeometry() { Name = "WorldMapDefault" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-8-min-J70" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-10-J70" }
        };

    private class LocalSource<T> : ISource
    {
        private readonly Func<List<T>> _func;

        public LocalSource(Func<List<T>> func)
        {
            _func = func;
        }

        public IList<object> GetValues() => _func.Invoke().Cast<object>().ToList();

        public async Task<IList<object>> GetValuesAsync() =>
            await Observable.Start(() => _func.Invoke().Cast<object>().ToList(), RxApp.TaskpoolScheduler);
    }
}
