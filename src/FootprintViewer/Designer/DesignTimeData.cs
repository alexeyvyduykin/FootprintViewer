using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using NetTopologySuite.Geometries;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nts = NetTopologySuite.Geometries;

namespace FootprintViewer.Designer
{
    public class DesignTimeData : IReadonlyDependencyResolver
    {
        private Mapsui.Map? _map;
        private ProjectFactory? _projectFactory;

        private SatelliteProvider? _satelliteProvider;
        private MapProvider? _mapProvider;
        private FootprintPreviewProvider? _footprintPreviewProvider;
        private FootprintPreviewGeometryProvider? _footprintPreviewGeometryProvider;
        private GroundTargetProvider? _groundTargetProvider;
        private FootprintProvider? _footprintProvider;
        private UserGeometryProvider? _userGeometryProvider;
        private GroundStationProvider? _groundStationProvider;

        private IFootprintLayerSource? _footprintLayerSource;
        private ISensorLayerSource? _sensorLayerSource;
        private ITargetLayerSource? _targetLayerSource;
        private ITrackLayerSource? _trackLayerSource;
        private IUserLayerSource? _userLayerSource;
        private IGroundStationLayerSource? _groundStationLayerSource;

        private SatelliteViewer? _satelliteViewer;
        private FootprintObserver? _footprintObserver;
        private GroundTargetViewer? _groundTargetViewer;
        private GroundStationViewer? _groundStationViewer;
        private SceneSearch? _sceneSearch;
        private MainViewModel? _mainViewModel;
        private SidePanel? _sidePanel;
        private CustomToolBar? _customToolBar;

        public object? GetService(Type? serviceType, string? contract = null)
        {
            if (serviceType == typeof(ProjectFactory))
            {
                return _projectFactory ??= new ProjectFactory(this);
            }
            else if (serviceType == typeof(Mapsui.Map))
            {
                return _map ??= new Mapsui.Map();
            }
            else if (serviceType == typeof(SatelliteProvider))
            {
                return _satelliteProvider ??= new DesignTimeSatelliteProvider();
            }
            else if (serviceType == typeof(ISensorLayerSource))
            {
                var provider = (SatelliteProvider)GetService(typeof(SatelliteProvider), contract)!;
                return _sensorLayerSource ??= new SensorLayerSource(provider);
            }
            else if (serviceType == typeof(ITrackLayerSource))
            {
                var provider = (SatelliteProvider)GetService(typeof(SatelliteProvider), contract)!;
                return _trackLayerSource ??= new TrackLayerSource(provider);
            }
            else if (serviceType == typeof(MapProvider))
            {
                return _mapProvider ??= new DesignTimeMapProvider();
            }
            else if (serviceType == typeof(FootprintPreviewProvider))
            {
                return _footprintPreviewProvider ??= new DesignTimeFootprintPreviewProvider();
            }
            else if (serviceType == typeof(FootprintPreviewGeometryProvider))
            {
                return _footprintPreviewGeometryProvider ??= new DesignTimeFootprintPreviewGeometryProvider();
            }
            else if (serviceType == typeof(GroundTargetProvider))
            {
                return _groundTargetProvider ??= new DesignDataGroundTargetProvider();
            }
            else if (serviceType == typeof(ITargetLayerSource))
            {
                var provider = (GroundTargetProvider)GetService(typeof(GroundTargetProvider), contract)!;
                return _targetLayerSource ??= new TargetLayerSource(provider);
            }
            else if (serviceType == typeof(FootprintProvider))
            {
                return _footprintProvider ??= new DesignDataFootprintProvider();
            }
            else if (serviceType == typeof(IFootprintLayerSource))
            {
                var provider = (FootprintProvider)GetService(typeof(FootprintProvider), contract)!;
                return _footprintLayerSource ??= new FootprintLayerSource(provider);
            }
            else if (serviceType == typeof(UserGeometryProvider))
            {
                return _userGeometryProvider ??= new DesignTimeUserGeometryProvider();
            }
            else if (serviceType == typeof(IUserLayerSource))
            {
                var provider = (UserGeometryProvider)GetService(typeof(UserGeometryProvider), contract)!;
                return _userLayerSource ??= new UserLayerSource(provider);
            }
            else if (serviceType == typeof(GroundStationProvider))
            {
                return _groundStationProvider ??= new DesignTimeGroundStationProvider();
            }
            else if (serviceType == typeof(IGroundStationLayerSource))
            {
                var provider = (GroundStationProvider)GetService(typeof(GroundStationProvider), contract)!;
                return _groundStationLayerSource ??= new GroundStationLayerSource(provider);
            }
            else if (serviceType == typeof(SatelliteViewer))
            {
                return _satelliteViewer ??= new SatelliteViewer(this);
            }
            else if (serviceType == typeof(GroundStationViewer))
            {
                return _groundStationViewer ??= new GroundStationViewer(this);
            }
            else if (serviceType == typeof(FootprintObserver))
            {
                return _footprintObserver ??= new FootprintObserver(this);
            }
            else if (serviceType == typeof(GroundTargetViewer))
            {
                return _groundTargetViewer ??= new GroundTargetViewer(this);
            }
            else if (serviceType == typeof(SceneSearch))
            {
                return _sceneSearch ??= new SceneSearch(this);
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

        public IEnumerable<object> GetServices(Type? serviceType, string? contract = null)
        {
            throw new Exception();
        }

        private class DesignTimeSatelliteProvider : SatelliteProvider
        {
            public DesignTimeSatelliteProvider() : base()
            {
                AddSource(new DesignTimeSatelliteSource());
            }

            private class DesignTimeSatelliteSource : Data.Sources.ISatelliteDataSource
            {
                private readonly IList<Satellite> _satellites;

                public DesignTimeSatelliteSource()
                {
                    _satellites = new List<Satellite>();

                    for (int i = 0; i < 5; i++)
                    {
                        _satellites.Add(DesignTimeSatelliteInfo.BuildModel());
                    }
                }

                public Task<List<SatelliteInfo>> GetSatelliteInfosAsync() =>
                    Task.Run(() => _satellites.Select(s => new SatelliteInfo(s)).ToList());
            }
        }

        private class DesignTimeGroundStationProvider : GroundStationProvider
        {
            public DesignTimeGroundStationProvider() : base()
            {
                AddSource(new DesignTimeGroundStationSource());
            }

            private class DesignTimeGroundStationSource : IDataSource<GroundStationInfo>
            {
                private readonly List<GroundStation> _groundStations;

                public DesignTimeGroundStationSource()
                {
                    _groundStations = new List<GroundStation>()
                    {
                        new GroundStation() { Name = "Москва",      Center = new nts.Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
                        new GroundStation() { Name = "Новосибирск", Center = new nts.Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
                        new GroundStation() { Name = "Хабаровск",   Center = new nts.Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
                        new GroundStation() { Name = "Шпицберген",  Center = new nts.Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
                        new GroundStation() { Name = "Анадырь",     Center = new nts.Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
                        new GroundStation() { Name = "Тикси",       Center = new nts.Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
                    };
                }

                public Task<List<GroundStationInfo>> GetValuesAsync(IFilter<GroundStationInfo>? filter = null) =>
                    Task.Run(() => _groundStations.Select(s => new GroundStationInfo(s)).ToList());
            }
        }

        private class DesignTimeFootprintPreviewProvider : FootprintPreviewProvider
        {
            public DesignTimeFootprintPreviewProvider() : base()
            {
                AddSource(new DesignTimeFootprintPreviewSource());
            }

            private class DesignTimeFootprintPreviewSource : Data.Sources.IFootprintPreviewDataSource
            {
                public IList<FootprintPreview> GetFootprintPreviews(IFilter<FootprintPreview>? filter)
                {
                    var list = new List<FootprintPreview>();

                    for (int i = 0; i < 8; i++)
                    {
                        list.Add(DesignTimeFootprintPreview.Build());
                    }

                    return list;
                }
            }
        }

        private class DesignTimeFootprintPreviewGeometryProvider : FootprintPreviewGeometryProvider
        {
            public DesignTimeFootprintPreviewGeometryProvider() : base() { }
        }

        private class DesignTimeMapProvider : MapProvider
        {
            public DesignTimeMapProvider() : base()
            {
                AddSource(new DesignTimeMapDataSource());
            }

            private class DesignTimeMapDataSource : Data.Sources.IMapDataSource
            {
                public IEnumerable<MapResource> GetMapResources()
                {
                    return new[]
                    {
                        new MapResource("WorldMapDefault", ""),
                        new MapResource("OAM-World-1-8-min-J70", ""),
                        new MapResource("OAM-World-1-10-J70", "")
                    };
                }
            }
        }

        private class DesignTimeUserGeometryProvider : UserGeometryProvider
        {
            public DesignTimeUserGeometryProvider() : base()
            {
                AddSource(new DesignTimeUserGeometrySource());
            }

            private class DesignTimeUserGeometrySource : Data.Sources.IUserGeometryDataSource
            {
                public Task AddAsync(UserGeometry geometry) => throw new NotImplementedException();

                private static List<UserGeometry> GetUserGeometries()
                {
                    var list = new List<UserGeometry>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(DesignTimeUserGeometryInfo.BuildModel());
                    }
                    return list;
                }

                public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(IFilter<UserGeometryInfo>? filter)
                {
                    await Task.Delay(2000);

                    return await Task.Run(() =>
                    {
                        return GetUserGeometries().Select(s => new UserGeometryInfo(s)).ToList();
                    });
                }

                public async Task RemoveAsync(UserGeometry geometry)
                {
                    await Task.Delay(1000);
                    throw new NotImplementedException();
                }

                public Task UpdateGeometry(string key, Geometry geometry)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private class DesignDataGroundTargetProvider : GroundTargetProvider
        {
            public DesignDataGroundTargetProvider() : base()
            {
                AddSource(new DesignTimeGroundTargetDataSource());
            }

            private class DesignTimeGroundTargetDataSource : Data.Sources.IGroundTargetDataSource
            {
                public List<GroundTarget> GetGroundTargets()
                {
                    var list = new List<GroundTarget>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(DesignTimeGroundTargetInfo.BuildModel());
                    }
                    return list;
                }

                public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(IFilter<GroundTargetInfo>? filter)
                {
                    await Task.Delay(2000);

                    return await Task.Run(() =>
                    {
                        if (filter == null)
                        {
                            return GetGroundTargets().Select(s => new GroundTargetInfo(s)).ToList();
                        }

                        return GetGroundTargets().Select(s => new GroundTargetInfo(s)).Where(s => filter.Filtering(s)).ToList();
                    });
                }
            }
        }

        private class DesignDataFootprintProvider : FootprintProvider
        {
            public DesignDataFootprintProvider() : base()
            {
                AddSource(new DesignTimeFootprintDataSource());
            }

            private class DesignTimeFootprintDataSource : IDataSource<FootprintInfo>
            {
                public List<Footprint> GetFootprints()
                {
                    var list = new List<Footprint>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(DesignTimeFootprintInfo.BuildModel());
                    }
                    return list;
                }

                public async Task<List<FootprintInfo>> GetValuesAsync(IFilter<FootprintInfo>? filter = null)
                {
                    await Task.Delay(2000);

                    return await Task.Run(() =>
                    {
                        return GetFootprints().Select(s => new FootprintInfo(s)).ToList();
                    });
                }
            }
        }
    }
}
