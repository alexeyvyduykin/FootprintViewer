using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Fluent.ViewModels.SidePanel;
using FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;
using FootprintViewer.Fluent.ViewModels.ToolBar;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.Designer;

public sealed class DesignDataDependencyResolver : IServiceProvider
{
    private Map? _map;
    private IMapNavigator? _mapNavigator;
    private AreaOfInterest? _areaOfInterest;
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

    public T GetService<T>()
    {
        return (T)GetService(typeof(T))!;
    }

    public object? GetService(Type? serviceType)
    {
        if (serviceType == typeof(IMap))
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
            return _groundTargetProvider ??= new GroundTargetProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(TrackProvider))
        {
            return _trackProvider ??= new TrackProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(SensorProvider))
        {
            return _sensorProvider ??= new SensorProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(GroundStationProvider))
        {
            return _groundStationProvider ??= new GroundStationProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(FootprintProvider))
        {
            return _footprintProvider ??= new FootprintProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(UserGeometryProvider))
        {
            return _userGeometryProvider ??= new UserGeometryProvider(GetService<IDataManager>(), GetService<LayerStyleManager>());
        }
        else if (serviceType == typeof(SatelliteTabViewModel))
        {
            return _satelliteTab ??= new SatelliteTabViewModel();// new SatelliteTabViewModel(this);
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
            return _footprintPreviewTab ??= new FootprintPreviewTabViewModel();
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
            return _languageManager ??= new LanguageManager(new[] { "en", "ru" });
        }
        throw new Exception();
    }

    private static Map CreateMap()
    {
        var map = new Map();
        map.AddLayer(new Layer(), LayerType.WorldMap);
        map.AddLayer(new Layer(), LayerType.FootprintImage); 
        map.AddLayer(new Layer(), LayerType.GroundStation);
        map.AddLayer(new Layer(), LayerType.GroundTarget);
        map.AddLayer(new Layer(), LayerType.Sensor);
        map.AddLayer(new Layer(), LayerType.Track);
        map.AddLayer(new Layer(), LayerType.Footprint);
        map.AddLayer(new Layer(), LayerType.FootprintImageBorder);
        map.AddLayer(new Layer(), LayerType.Edit);
        map.AddLayer(new Layer(), LayerType.Vertex);
        map.AddLayer(new Layer(), LayerType.User);
        return map;
    }

    private static DataManager CreateDataManager()
    {
        var source5 = new LocalSource<UserGeometry>(BuildUserGeometries);
        var source6 = new LocalSource<MapResource>(BuildMapResources);
        var source7 = new LocalSource<FootprintPreview>(BuildFootprintPreviews);
        var source8 = new LocalSource<FootprintPreviewGeometry>(BuildFootprintPreviewGeometries);
        var source9 = new LocalSource<PlannedScheduleResult>(BuildPlannedSchedule);

        var dir = Directory.GetCurrentDirectory();

        var filesource1 = new FileSource(new[] { Path.Combine(dir, "map_topo_4343.mbtiles") }, MapResource.Build);
        var filesource2 = new FileSource(new[] { Path.Combine(dir, "world.mbtiles") }, MapResource.Build);
        var filesource3 = new FileSource(new[] { Path.Combine(dir, "WorlMapWithCountryBorders.mbtiles") }, MapResource.Build);
        var filesource4 = new FileSource(new[] { Path.Combine(dir, "MapBackground_Mercator.mbtiles") }, MapResource.Build);

        var sources = new Dictionary<string, IList<ISource>>()
        {
            { DbKeys.UserGeometries.ToString(), new[] { source5 } },
            { DbKeys.Maps.ToString(), new[] { filesource1, filesource2, filesource3, filesource4 } },
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

    private static List<PlannedScheduleResult> BuildPlannedSchedule() => Build(1, PlannedScheduleBuilder.CreateRandom);

    private static List<UserGeometry> BuildUserGeometries() => Build(10, UserGeometryBuilder.CreateRandom);

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
