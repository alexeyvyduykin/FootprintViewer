using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using FootprintViewer.ViewModels.TimelinePanel;
using FootprintViewer.ViewModels.ToolBar;
using Mapsui;
using Mapsui.Layers;
using NetTopologySuite.Geometries;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private GroundTargetTabViewModel? _groundTargetTab;
    private GroundStationTabViewModel? _groundStationTab;
    private UserGeometryTabViewModel? _userGeometryTab;
    private FootprintPreviewTabViewModel? _footprintPreviewTab;
    private MainViewModel? _mainViewModel;
    private SidePanelViewModel? _sidePanel;
    private CustomToolBarViewModel? _customToolBar;
    private TimelinePanelViewModel? _timelinePanel;
    private IDataManager? _dataManager;
    private ILanguageManager? _languageManager;
    private FeatureManager? _featureManager;
    private LayerStyleManager? _layerStyleManager;

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
        else if (serviceType == typeof(CustomToolBarViewModel))
        {
            return _customToolBar ??= new CustomToolBarViewModel(this);
        }
        else if (serviceType == typeof(SidePanelViewModel))
        {
            return _sidePanel ??= new SidePanelViewModel();
        }
        else if (serviceType == typeof(TimelinePanelViewModel))
        {
            return _timelinePanel ??= new TimelinePanelViewModel(this);
        }
        else if (serviceType == typeof(MainViewModel))
        {
            return _mainViewModel ??= new MainViewModel(this);
        }
        else if (serviceType == typeof(IDataManager))
        {
            return _dataManager ??= new DesignTimeRepository();
        }
        else if (serviceType == typeof(ILanguageManager))
        {
            return _languageManager ??= new LanguageManager(new LanguagesConfiguration() { AvailableLocales = new[] { "en", "ru" } });
        }
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

    public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
    {
        throw new Exception();
    }

    private class DesignTimeRepository : DataManager
    {
        private static readonly Dictionary<string, IList<ISource>> _sources;

        static DesignTimeRepository()
        {
            var source1 = new LocalSource<Footprint>(Task.Run(() => BuildFootprints()));
            var source2 = new LocalSource<GroundTarget>(Task.Run(() => BuildGroundTargets()));
            var source3 = new LocalSource<Satellite>(Task.Run(() => BuildSatellites()));
            var source4 = new LocalSource<GroundStation>(Task.Run(() => BuildGroundStations()));
            var source5 = new LocalSource<UserGeometry>(Task.Run(() => BuildUserGeometries()));
            var source6 = new LocalSource<MapResource>(Task.Run(() => BuildMapResources()));
            var source7 = new LocalSource<FootprintPreview>(Task.Run(() => BuildFootprintPreviews()));
            var source8 = new LocalSource<FootprintPreviewGeometry>(Task.Run(() => BuildFootprintPreviewGeometries()));

            _sources = new Dictionary<string, IList<ISource>>()
            {
                { DbKeys.Footprints.ToString(), new[] { source1 } },
                { DbKeys.GroundTargets.ToString(), new[] { source2 } },
                { DbKeys.Satellites.ToString(), new[] { source3 } },
                { DbKeys.GroundStations.ToString(), new[] { source4 } },
                { DbKeys.UserGeometries.ToString(), new[] { source5 } },
                { DbKeys.Maps.ToString(), new[] { source6 } },
                { DbKeys.FootprintPreviews.ToString(), new[] { source7 } },
                { DbKeys.FootprintPreviewGeometries.ToString(), new[] { source8 } }
            };
        }

        public DesignTimeRepository() : base(_sources)
        {

        }

        private static List<Satellite> BuildSatellites() =>
            new int[5].Select(_ => RandomModelBuilder.BuildSatellite()).ToList();
        private static List<Footprint> BuildFootprints() =>
            new int[10].Select(_ => RandomModelBuilder.BuildFootprint()).ToList();

        private static List<GroundTarget> BuildGroundTargets() =>
            new int[10].Select(_ => RandomModelBuilder.BuildGroundTarget()).ToList();

        private static List<UserGeometry> BuildUserGeometries() =>
            new int[10].Select(_ => RandomModelBuilder.BuildUserGeometry()).ToList();

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

        private static List<FootprintPreview> BuildFootprintPreviews() =>
            new int[8].Select(_ => RandomModelBuilder.BuildFootprintPreview()).ToList();

        private static List<FootprintPreviewGeometry> BuildFootprintPreviewGeometries() =>
            new()
            {
                new FootprintPreviewGeometry() { Name = "WorldMapDefault" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-8-min-J70" },
                new FootprintPreviewGeometry() { Name = "OAM-World-1-10-J70" }
            };
    }

    private class LocalSource<T> : ISource
    {
        private List<T>? _list;
        private readonly Task<List<T>> _task;

        public LocalSource(Task<List<T>> task)
        {
            _task = task;
        }

        public IList<object> GetValues()
        {
            _list ??= _task.Result;
            return _list.Cast<object>().ToList();
        }

        public async Task<IList<object>> GetValuesAsync() => await Task.Run(async () =>
        {
            _list ??= await _task;
            return _list.Cast<object>().ToList();
        });
    }
}
