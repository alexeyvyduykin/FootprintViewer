using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using NetTopologySuite.Geometries;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeData : IReadonlyDependencyResolver
    {
        private Map? _map;
        private IMapNavigator? _mapNavigator;
        private ProjectFactory? _projectFactory;
        private ViewModelFactory? _viewModelFactory;
        private ITargetLayerSource? _targetLayerSource;
        private SatelliteTab? _satelliteTab;
        private FootprintTab? _footprintTab;
        private GroundTargetTab? _groundTargetTab;
        private GroundStationTab? _groundStationTab;
        private UserGeometryTab? _userGeometryTab;
        private FootprintPreviewTab? _footprintPreviewTab;
        private MainViewModel? _mainViewModel;
        private SidePanel? _sidePanel;
        private CustomToolBar? _customToolBar;
     //   private MapBackgroundList? _mapBackgroundList;
        private IDataManager? _dataManager;
        private ILanguageManager? _languageManager;

        public object? GetService(Type? serviceType, string? contract = null)
        {
            if (serviceType == typeof(ProjectFactory))
            {
                return _projectFactory ??= new ProjectFactory(this);
            }
            else if (serviceType == typeof(ViewModelFactory))
            {
                return _viewModelFactory ??= new ViewModelFactory(this);
            }
            else if (serviceType == typeof(IMap))
            {
                return _map ??= CreateMap();
            }
            else if (serviceType == typeof(IMapNavigator))
            {
                return _mapNavigator ??= new MapNavigator();
            }
            //else if (serviceType == typeof(MapBackgroundList))
            //{
            //    return _mapBackgroundList ??= new MapBackgroundList();
            //}
            else if (serviceType == typeof(ITargetLayerSource))
            {
                return _targetLayerSource ??= new TargetLayerSource();
            }
            else if (serviceType == typeof(SatelliteTab))
            {
                return _satelliteTab ??= new SatelliteTab(this);
            }
            else if (serviceType == typeof(GroundStationTab))
            {
                return _groundStationTab ??= new GroundStationTab(this);
            }
            else if (serviceType == typeof(FootprintTab))
            {
                return _footprintTab ??= new FootprintTab(this);
            }
            else if (serviceType == typeof(GroundTargetTab))
            {
                return _groundTargetTab ??= new GroundTargetTab(this);
            }
            else if (serviceType == typeof(FootprintPreviewTab))
            {
                return _footprintPreviewTab ??= new FootprintPreviewTab(this);
            }
            else if (serviceType == typeof(UserGeometryTab))
            {
                return _userGeometryTab ??= new UserGeometryTab(this);
            }
            else if (serviceType == typeof(CustomToolBar))
            {
                return _customToolBar ??= new CustomToolBar(this);
            }
            else if (serviceType == typeof(SidePanel))
            {
                return _sidePanel ??= new SidePanel();
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
            map.AddLayer(new GroundStationLayer(), LayerType.GroundStation);
            map.AddLayer(new TrackLayer(), LayerType.Track);
            map.AddLayer(new SensorLayer(), LayerType.Sensor);
            map.AddLayer(new Layer(), LayerType.User);
            return map;
        }

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            throw new Exception();
        }

        private class DesignTimeRepository : DataManager
        {
            public DesignTimeRepository()
            {
                var source1 = new LocalSource<Footprint>(Task.Run(() => BuildFootprints()));
                var source2 = new LocalSource<GroundTarget>(Task.Run(() => BuildGroundTargets()));
                var source3 = new LocalSource<Satellite>(Task.Run(() => BuildSatellites()));
                var source4 = new LocalSource<GroundStation>(Task.Run(() => BuildGroundStations()));
                var source5 = new LocalSource<UserGeometry>(Task.Run(() => BuildUserGeometries()));
                var source6 = new LocalSource<MapResource>(Task.Run(() => BuildMapResources()));
                var source7 = new LocalSource<FootprintPreview>(Task.Run(() => BuildFootprintPreviews()));
                var source8 = new LocalSource<FootprintPreviewGeometry>(Task.Run(() => BuildFootprintPreviewGeometries()));

                RegisterSource(DbKeys.Footprints.ToString(), source1);
                RegisterSource(DbKeys.GroundTargets.ToString(), source2);
                RegisterSource(DbKeys.Satellites.ToString(), source3);
                RegisterSource(DbKeys.GroundStations.ToString(), source4);
                RegisterSource(DbKeys.UserGeometries.ToString(), source5);
                RegisterSource(DbKeys.Maps.ToString(), source6);
                RegisterSource(DbKeys.FootprintPreviews.ToString(), source7);
                RegisterSource(DbKeys.FootprintPreviewGeometries.ToString(), source8);
            }

            private static List<Satellite> BuildSatellites() =>
                new int[5].Select(_ => DesignTimeSatelliteViewModel.BuildModel()).ToList();
            private static List<Footprint> BuildFootprints() =>
                new int[10].Select(_ => DesignTimeFootprintViewModel.BuildModel()).ToList();

            private static List<GroundTarget> BuildGroundTargets() =>
                new int[10].Select(_ => DesignTimeGroundTargetViewModel.BuildModel()).ToList();

            private static List<UserGeometry> BuildUserGeometries() =>
                new int[10].Select(_ => DesignTimeUserGeometryViewModel.BuildModel()).ToList();

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
                new int[8].Select(_ => DesignTimeFootprintPreviewViewModel.BuildModel()).ToList();

            private static List<FootprintPreviewGeometry> BuildFootprintPreviewGeometries() =>
                new()
                {
                    new FootprintPreviewGeometry() { Name = "WorldMapDefault" },
                    new FootprintPreviewGeometry() { Name = "OAM-World-1-8-min-J70" },
                    new FootprintPreviewGeometry() { Name = "OAM-World-1-10-J70" }
                };
        }

        internal class LocalSource<T> : ISource
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
}
