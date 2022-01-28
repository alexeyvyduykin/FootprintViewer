﻿using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using NetTopologySuite.Geometries;
using Splat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Designer
{
    public class DesignTimeData : IReadonlyDependencyResolver
    {
        private readonly Mapsui.Map _map = new Mapsui.Map();
        private readonly ProjectFactory _projectFactory = new ProjectFactory();
        private readonly IDataSource _dataSource = new DesignTimeDataSource();
        private readonly IFootprintDataSource _footprintDataSource = new FootprintDataSource();
        private readonly TargetLayer _groundTargetDataSource = new DesignTimeTargetLayer();
        private readonly MapProvider _mapProvider = new DesignTimeMapProvider();
        private readonly FootprintPreviewProvider _footprintPreviewProvider = new DesignTimeFootprintPreviewProvider();
        private readonly FootprintPreviewGeometryProvider _footprintPreviewGeometryProvider = new DesignTimeFootprintPreviewGeometryProvider();
        private readonly GroundTargetProvider _groundTargetProvider = new DesignDataGroundTargetProvider();
        private SatelliteViewer? _satelliteViewer;
        private FootprintObserver? _footprintObserver;
        private GroundTargetViewer? _groundTargetViewer;
        private SceneSearch? _sceneSearch;
        private MainViewModel? _mainViewModel;
        private SidePanel? _sidePanel;
        private ToolBar? _toolBar;

        public object? GetService(Type? serviceType, string? contract = null)
        {
            if (serviceType == typeof(ProjectFactory))
            {
                return _projectFactory;
            }
            else if (serviceType == typeof(Mapsui.Map))
            {
                return _map;//_projectFactory.CreateMap(this);
            }
            else if (serviceType == typeof(IDataSource))
            {
                return _dataSource;
            }
            else if (serviceType == typeof(MapProvider))
            {
                return _mapProvider;
            }
            else if (serviceType == typeof(FootprintPreviewProvider))
            {
                return _footprintPreviewProvider;
            }
            else if (serviceType == typeof(FootprintPreviewGeometryProvider))
            {
                return _footprintPreviewGeometryProvider;
            }
            else if (serviceType == typeof(GroundTargetProvider))
            {
                return _groundTargetProvider;
            }
            else if (serviceType == typeof(SatelliteViewer))
            {
                return _satelliteViewer ??= new SatelliteViewer(this);
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
            else if (serviceType == typeof(ToolBar))
            {
                return _toolBar ??= new ToolBar(this);
            }
            else if (serviceType == typeof(SidePanel))
            {
                return _sidePanel ??= new SidePanel();
            }
            else if (serviceType == typeof(IFootprintDataSource))
            {
                return _footprintDataSource;
            }
            else if (serviceType == typeof(TargetLayer))
            {
                return _groundTargetDataSource;
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

        private class DesignTimeDataSource : IDataSource
        {
            private readonly IList<Satellite> _satellites;
            private readonly IList<Footprint> _footprints;
            private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _leftStrips;
            private readonly IDictionary<string, Dictionary<int, List<List<Point>>>> _rightStrips;
            private readonly IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> _groundTracks;

            public DesignTimeDataSource()
            {
                var dt = new DateTime(2000, 6, 1, 12, 0, 0);

                _satellites = new List<Satellite>();

                for (int i = 0; i < 5; i++)
                {
                    var sat = new Satellite()
                    {
                        Name = $"Satellite{i + 1}",
                        Semiaxis = 6945.03,
                        Eccentricity = 0.0,
                        InclinationDeg = 97.65,
                        ArgumentOfPerigeeDeg = 0.0,
                        LongitudeAscendingNodeDeg = 0.0,
                        RightAscensionAscendingNodeDeg = 0.0,
                        Period = 5760.0,
                        Epoch = dt,
                        InnerHalfAngleDeg = 32,
                        OuterHalfAngleDeg = 48
                    };

                    _satellites.Add(sat);
                }

                _footprints = new List<Footprint>();

                for (int i = 0; i < 3; i++)
                {
                    var footprint = new Footprint()
                    {
                        Name = $"Footrpint000{i + 1}",
                        SatelliteName = "Satellite1",
                        Center = new Point(54.434545, -12.435454),
                        Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                        Duration = 35,
                        Node = 11,
                        Direction = SatelliteStripDirection.Left,
                    };

                    _footprints.Add(footprint);
                }

                _leftStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();
                _rightStrips = new Dictionary<string, Dictionary<int, List<List<Point>>>>();
                _groundTracks = new Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>>();
            }

            public IEnumerable<Satellite> Satellites => _satellites;
            public IEnumerable<Footprint> Footprints => _footprints;
            public IDictionary<string, Dictionary<int, List<List<Point>>>> LeftStrips => _leftStrips;
            public IDictionary<string, Dictionary<int, List<List<Point>>>> RightStrips => _rightStrips;
            public IEnumerable<GroundTarget> Targets => new List<GroundTarget>();
            public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks => _groundTracks;
        }

        private class DesignTimeFootprintPreviewProvider : FootprintPreviewProvider
        {
            public DesignTimeFootprintPreviewProvider() : base() { }
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

        private class FootprintDataSource : IFootprintDataSource
        {
            private readonly List<Footprint> _footprints;

            public FootprintDataSource()
            {
                _footprints = new List<Footprint>()
                {
                    new Footprint()
                    {
                        Name = "Footrpint0001",
                        SatelliteName = "Satellite1",
                        Center = new Point(54.434545, -12.435454),
                        Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                        Duration = 35,
                        Node = 11,
                        Direction = SatelliteStripDirection.Left,
                    },

                    new Footprint()
                    {
                        Name = "Footrpint0002",
                        SatelliteName = "Satellite1",
                        Center = new Point(54.434545, -12.435454),
                        Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                        Duration = 35,
                        Node = 11,
                        Direction = SatelliteStripDirection.Left,
                    },
                   new Footprint()
                   {
                       Name = "Footrpint0003",
                       SatelliteName = "Satellite1",
                       Center = new Point(54.434545, -12.435454),
                       Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                       Duration = 35,
                       Node = 11,
                       Direction = SatelliteStripDirection.Left,
                   },
                };
            }

            public List<Footprint> GetFootprints() => _footprints;

            public Task<List<Footprint>> GetFootprintsAsync() => Task.Run(() => _footprints);
        }

        private class DesignTimeTargetLayer : Layers.TargetLayer
        {         
            public DesignTimeTargetLayer() : base(new TargetProvider(new DesignDataGroundTargetProvider()))
            {

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
                public IEnumerable<GroundTarget> GetGroundTargets()
                {
                    return new List<GroundTarget>()
                    {
                        new GroundTarget()
                        {
                            Name = "GroundTarget0001",
                            Type = GroundTargetType.Point,
                        },

                    new GroundTarget()
                    {
                        Name = "GroundTarget0002",
                        Type = GroundTargetType.Route,
                    },
                   new GroundTarget()
                   {
                       Name = "GroundTarget0003",
                       Type = GroundTargetType.Area,
                   },
                };
                }
            }
        }
    }
}
