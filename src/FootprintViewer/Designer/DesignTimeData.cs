﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
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

        private IProvider<SatelliteInfo>? _satelliteProvider;
        private IProvider<MapResource>? _mapProvider;
        private IProvider<FootprintPreview>? _footprintPreviewProvider;
        private IProvider<(string, nts.Geometry)>? _footprintPreviewGeometryProvider;
        private IProvider<GroundTargetInfo>? _groundTargetProvider;
        private IProvider<FootprintInfo>? _footprintProvider;
        private IEditableProvider<UserGeometryInfo>? _userGeometryProvider;
        private IProvider<GroundStationInfo>? _groundStationProvider;

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
            else if (serviceType == typeof(Provider<SatelliteInfo>))
            {
                return _satelliteProvider ??= new DesignTimeSatelliteProvider();
            }
            else if (serviceType == typeof(ISensorLayerSource))
            {
                var provider = (IProvider<SatelliteInfo>)GetService(typeof(Provider<SatelliteInfo>), contract)!;
                return _sensorLayerSource ??= new SensorLayerSource(provider);
            }
            else if (serviceType == typeof(ITrackLayerSource))
            {
                var provider = (IProvider<SatelliteInfo>)GetService(typeof(Provider<SatelliteInfo>), contract)!;
                return _trackLayerSource ??= new TrackLayerSource(provider);
            }
            else if (serviceType == typeof(Provider<MapResource>))
            {
                return _mapProvider ??= new DesignTimeMapProvider();
            }
            else if (serviceType == typeof(Provider<FootprintPreview>))
            {
                return _footprintPreviewProvider ??= new DesignTimeFootprintPreviewProvider();
            }
            else if (serviceType == typeof(Provider<(string, nts.Geometry)>))
            {
                return _footprintPreviewGeometryProvider ??= new DesignTimeFootprintPreviewGeometryProvider();
            }
            else if (serviceType == typeof(Provider<GroundTargetInfo>))
            {
                return _groundTargetProvider ??= new DesignDataGroundTargetProvider();
            }
            else if (serviceType == typeof(ITargetLayerSource))
            {
                var provider = (IProvider<GroundTargetInfo>)GetService(typeof(Provider<GroundTargetInfo>), contract)!;
                return _targetLayerSource ??= new TargetLayerSource(provider);
            }
            else if (serviceType == typeof(Provider<FootprintInfo>))
            {
                return _footprintProvider ??= new DesignDataFootprintProvider();
            }
            else if (serviceType == typeof(IFootprintLayerSource))
            {
                var provider = (IProvider<FootprintInfo>)GetService(typeof(Provider<FootprintInfo>), contract)!;
                return _footprintLayerSource ??= new FootprintLayerSource(provider);
            }
            else if (serviceType == typeof(EditableProvider<UserGeometryInfo>))
            {
                return _userGeometryProvider ??= new DesignTimeUserGeometryProvider();
            }
            else if (serviceType == typeof(IUserLayerSource))
            {
                var provider = (IEditableProvider<UserGeometryInfo>)GetService(typeof(EditableProvider<UserGeometryInfo>), contract)!;
                return _userLayerSource ??= new UserLayerSource(provider);
            }
            else if (serviceType == typeof(Provider<GroundStationInfo>))
            {
                return _groundStationProvider ??= new DesignTimeGroundStationProvider();
            }
            else if (serviceType == typeof(IGroundStationLayerSource))
            {
                var provider = (IProvider<GroundStationInfo>)GetService(typeof(Provider<GroundStationInfo>), contract)!;
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

        private class DesignTimeSatelliteProvider : Provider<SatelliteInfo>
        {
            public DesignTimeSatelliteProvider() : base()
            {
                AddSource(new DesignTimeSatelliteSource());
            }

            private class DesignTimeSatelliteSource : IDataSource<SatelliteInfo>
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

                public Task<List<SatelliteInfo>> GetValuesAsync(IFilter<SatelliteInfo>? filter = null) =>
                    Task.Run(() => _satellites.Select(s => new SatelliteInfo(s)).ToList());
            }
        }

        private class DesignTimeGroundStationProvider : Provider<GroundStationInfo>
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

        private class DesignTimeFootprintPreviewProvider : Provider<FootprintPreview>
        {
            public DesignTimeFootprintPreviewProvider() : base()
            {
                AddSource(new DesignTimeFootprintPreviewSource());
            }

            private class DesignTimeFootprintPreviewSource : IDataSource<FootprintPreview>
            {
                public async Task<List<FootprintPreview>> GetValuesAsync(IFilter<FootprintPreview>? filter = null)
                {
                    return await Task.Run(() =>
                    {
                        var list = new List<FootprintPreview>();

                        for (int i = 0; i < 8; i++)
                        {
                            list.Add(DesignTimeFootprintPreview.Build());
                        }

                        return list;
                    });
                }
            }
        }

        private class DesignTimeFootprintPreviewGeometryProvider : Provider<(string, nts.Geometry)>
        {
            public DesignTimeFootprintPreviewGeometryProvider() : base() { }
        }

        private class DesignTimeMapProvider : Provider<MapResource>
        {
            public DesignTimeMapProvider() : base()
            {
                AddSource(new DesignTimeMapDataSource());
            }

            private class DesignTimeMapDataSource : IDataSource<MapResource>
            {
                public async Task<List<MapResource>> GetValuesAsync(IFilter<MapResource>? filter = null)
                {
                    return await Task.Run(() =>
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

        private class DesignTimeUserGeometryProvider : EditableProvider<UserGeometryInfo>
        {
            public DesignTimeUserGeometryProvider() : base()
            {
                AddSource(new DesignTimeUserGeometrySource());
            }

            private class DesignTimeUserGeometrySource : IEditableDataSource<UserGeometryInfo>
            {
                public Task AddAsync(UserGeometryInfo geometry) => throw new NotImplementedException();

                private static List<UserGeometry> GetUserGeometries()
                {
                    var list = new List<UserGeometry>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(DesignTimeUserGeometryInfo.BuildModel());
                    }
                    return list;
                }

                public async Task<List<UserGeometryInfo>> GetValuesAsync(IFilter<UserGeometryInfo>? filter)
                {
                    await Task.Delay(2000);

                    return await Task.Run(() =>
                    {
                        return GetUserGeometries().Select(s => new UserGeometryInfo(s)).ToList();
                    });
                }

                public Task RemoveAsync(UserGeometryInfo geometry) => throw new NotImplementedException();

                public Task EditAsync(string key, UserGeometryInfo geometry) => throw new NotImplementedException();
            }
        }

        private class DesignDataGroundTargetProvider : Provider<GroundTargetInfo>
        {
            public DesignDataGroundTargetProvider() : base()
            {
                AddSource(new DesignTimeGroundTargetDataSource());
            }

            private class DesignTimeGroundTargetDataSource : IDataSource<GroundTargetInfo>
            {
                public static List<GroundTarget> GetGroundTargets()
                {
                    var list = new List<GroundTarget>();
                    for (int i = 0; i < 10; i++)
                    {
                        list.Add(DesignTimeGroundTargetInfo.BuildModel());
                    }
                    return list;
                }

                public async Task<List<GroundTargetInfo>> GetValuesAsync(IFilter<GroundTargetInfo>? filter)
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

        private class DesignDataFootprintProvider : Provider<FootprintInfo>
        {
            public DesignDataFootprintProvider() : base()
            {
                AddSource(new DesignTimeFootprintDataSource());
            }

            private class DesignTimeFootprintDataSource : IDataSource<FootprintInfo>
            {
                public static List<Footprint> GetFootprints()
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
