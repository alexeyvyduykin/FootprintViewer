using FootprintViewer.Data;
using FootprintViewer.Layers;
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
        private IProvider<Satellite>? _satelliteProvider;
        private IProvider<MapResource>? _mapProvider;
        private IProvider<FootprintPreview>? _footprintPreviewProvider;
        private IProvider<(string, Geometry)>? _footprintPreviewGeometryProvider;
        private IProvider<GroundTarget>? _groundTargetProvider;
        private IProvider<Footprint>? _footprintProvider;
        private IEditableProvider<UserGeometry>? _userGeometryProvider;
        private IProvider<GroundStation>? _groundStationProvider;
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
        private MapBackgroundList? _mapBackgroundList;

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
            else if (serviceType == typeof(IProvider<Satellite>))
            {
                return _satelliteProvider ??= new DesignTimeSatelliteProvider();
            }
            else if (serviceType == typeof(MapBackgroundList))
            {
                return _mapBackgroundList ??= new MapBackgroundList();
            }
            else if (serviceType == typeof(IProvider<MapResource>))
            {
                return _mapProvider ??= new DesignTimeMapProvider();
            }
            else if (serviceType == typeof(IProvider<FootprintPreview>))
            {
                return _footprintPreviewProvider ??= new DesignTimeFootprintPreviewProvider();
            }
            else if (serviceType == typeof(IProvider<(string, Geometry)>))
            {
                return _footprintPreviewGeometryProvider ??= new DesignTimeFootprintPreviewGeometryProvider();
            }
            else if (serviceType == typeof(IProvider<GroundTarget>))
            {
                return _groundTargetProvider ??= new DesignDataGroundTargetProvider();
            }
            else if (serviceType == typeof(ITargetLayerSource))
            {
                return _targetLayerSource ??= new TargetLayerSource();
            }
            else if (serviceType == typeof(IProvider<Footprint>))
            {
                return _footprintProvider ??= new DesignDataFootprintProvider();
            }
            else if (serviceType == typeof(IEditableProvider<UserGeometry>))
            {
                return _userGeometryProvider ??= new DesignTimeUserGeometryProvider();
            }
            else if (serviceType == typeof(IProvider<GroundStation>))
            {
                return _groundStationProvider ??= new DesignTimeGroundStationProvider();
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

        private class DesignTimeSatelliteProvider : Provider<Satellite>
        {
            public DesignTimeSatelliteProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeSatelliteDataManager() });
            }

            private class DesignTimeSatelliteDataManager : BaseDataManager<Satellite, IDesignTimeSource>
            {
                private List<Satellite>? _satellites;

                protected override async Task<List<Satellite>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<Satellite>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _satellites ??= await Build();

                        return _satellites;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<Satellite, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _satellites ??= await Build();

                        return _satellites.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<Satellite>> Build()
                {
                    return Task.Run(() =>
                    {
                        return new List<Satellite>()
                        {
                            DesignTimeSatelliteViewModel.BuildModel(),
                            DesignTimeSatelliteViewModel.BuildModel(),
                            DesignTimeSatelliteViewModel.BuildModel(),
                            DesignTimeSatelliteViewModel.BuildModel(),
                            DesignTimeSatelliteViewModel.BuildModel(),
                        };
                    });
                }
            }
        }

        private class DesignTimeGroundStationProvider : Provider<GroundStation>
        {
            public DesignTimeGroundStationProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeGroundStationDataManager() });
            }

            private class DesignTimeGroundStationDataManager : BaseDataManager<GroundStation, IDesignTimeSource>
            {
                private List<GroundStation>? _groundStations;

                protected override async Task<List<GroundStation>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<GroundStation>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _groundStations ??= await Build();

                        return _groundStations;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<GroundStation, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _groundStations ??= await Build();

                        return _groundStations.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<GroundStation>> Build()
                {
                    return Task.Run(() =>
                    {
                        return new List<GroundStation>()
                        {
                            new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                            new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                            new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                            new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                            new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                            new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
                        };
                    });
                }
            }
        }

        private class DesignTimeFootprintPreviewProvider : Provider<FootprintPreview>
        {
            public DesignTimeFootprintPreviewProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeFootprintPreviewDataManager() });
            }

            private class DesignTimeFootprintPreviewDataManager : BaseDataManager<FootprintPreview, IDesignTimeSource>
            {
                private List<FootprintPreview>? _footprints;

                protected override async Task<List<FootprintPreview>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<FootprintPreview>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _footprints ??= await Build();

                        return _footprints;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<FootprintPreview, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _footprints ??= await Build();

                        return _footprints.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<FootprintPreview>> Build()
                {
                    return Task.Run(() =>
                    {
                        var list = new List<FootprintPreview>();

                        for (int i = 0; i < 8; i++)
                        {
                            list.Add(DesignTimeFootprintPreviewViewModel.BuildModel());
                        }

                        return list;
                    });
                }
            }
        }

        private class DesignTimeFootprintPreviewGeometryProvider : Provider<(string, Geometry)>
        {
            public DesignTimeFootprintPreviewGeometryProvider() : base() { }
        }

        private class DesignTimeMapProvider : Provider<MapResource>
        {
            public DesignTimeMapProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeMapResourceDataManager() });
            }

            private class DesignTimeMapResourceDataManager : BaseDataManager<MapResource, IDesignTimeSource>
            {
                private List<MapResource>? _maps;

                protected override async Task<List<MapResource>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<MapResource>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _maps ??= await Build();

                        return _maps;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<MapResource, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _maps ??= await Build();

                        return _maps.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<MapResource>> Build()
                {
                    return Task.Run(() =>
                    {
                        return new List<MapResource>()
                        {
                            new MapResource("WorldMapDefault", ""),
                            new MapResource("OAM-World-1-8-min-J70", ""),
                            new MapResource("OAM-World-1-10-J70", "")
                        };
                    });
                }
            }
        }

        private class DesignTimeUserGeometryProvider : EditableProvider<UserGeometry>
        {
            public DesignTimeUserGeometryProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeUserGeometryDataManger() });
            }

            private class DesignTimeUserGeometryDataManger : BaseDataManager<UserGeometry, IDesignTimeSource>
            {
                private List<UserGeometry>? _userGeometries;

                protected override async Task<List<UserGeometry>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<UserGeometry>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _userGeometries ??= await Build();

                        return _userGeometries;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<UserGeometry, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _userGeometries ??= await Build();

                        return _userGeometries.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<UserGeometry>> Build()
                {
                    return Task.Run(() =>
                    {
                        var list = new List<UserGeometry>();
                        for (int i = 0; i < 10; i++)
                        {
                            list.Add(DesignTimeUserGeometryViewModel.BuildModel());
                        }
                        return list;
                    });
                }
            }
        }

        private class DesignDataGroundTargetProvider : Provider<GroundTarget>
        {
            public DesignDataGroundTargetProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeGroundTargetDataManager() });
            }

            private class DesignTimeGroundTargetDataManager : BaseDataManager<GroundTarget, IDesignTimeSource>
            {
                private List<GroundTarget>? _groundTargets;

                protected override async Task<List<GroundTarget>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<GroundTarget>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _groundTargets ??= await Build();

                        return _groundTargets;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<GroundTarget, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _groundTargets ??= await Build();

                        return _groundTargets.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<GroundTarget>> Build()
                {
                    return Task.Run(() =>
                    {
                        var list = new List<GroundTarget>();
                        for (int i = 0; i < 10; i++)
                        {
                            list.Add(DesignTimeGroundTargetViewModel.BuildModel());
                        }
                        return list;
                    });
                }
            }
        }

        private class DesignDataFootprintProvider : Provider<Footprint>
        {
            public DesignDataFootprintProvider() : base()
            {
                AddSources(new[] { new DesignTimeDataSource() });
                AddManagers(new[] { new DesignTimeFootprintDataManager() });
            }

            private class DesignTimeFootprintDataManager : BaseDataManager<Footprint, IDesignTimeSource>
            {
                private List<Footprint>? _footprints;

                protected override async Task<List<Footprint>> GetNativeValuesAsync(IDesignTimeSource dataSource, IFilter<Footprint>? filter)
                {
                    return await Task.Run(async () =>
                    {
                        _footprints ??= await Build();

                        return _footprints;
                    });
                }

                protected override async Task<List<T>> GetValuesAsync<T>(IDesignTimeSource dataSource, IFilter<T>? filter, Func<Footprint, T> converter)
                {
                    return await Task.Run(async () =>
                    {
                        _footprints ??= await Build();

                        return _footprints.Select(s => converter(s)).ToList();
                    });
                }

                public static Task<List<Footprint>> Build()
                {
                    return Task.Run(() =>
                    {
                        var list = new List<Footprint>();
                        for (int i = 0; i < 10; i++)
                        {
                            list.Add(DesignTimeFootprintViewModel.BuildModel());
                        }
                        return list;
                    });
                }
            }
        }

        private interface IDesignTimeSource : IDataSource
        {

        }

        private class DesignTimeDataSource : IDesignTimeSource
        {
            public bool Equals(IDataSource? other) => true;
        }
    }
}
