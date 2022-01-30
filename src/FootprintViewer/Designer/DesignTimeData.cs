using FootprintViewer.Data;
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
        private Mapsui.Map? _map;
        private ProjectFactory? _projectFactory;
        private IDataSource? _dataSource;
        private IFootprintDataSource? _footprintDataSource;
        private TargetLayer? _groundTargetDataSource;
        private MapProvider? _mapProvider;
        private FootprintPreviewProvider? _footprintPreviewProvider;
        private FootprintPreviewGeometryProvider? _footprintPreviewGeometryProvider;
        private GroundTargetProvider? _groundTargetProvider;
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
                return _projectFactory ??= new ProjectFactory();
            }
            else if (serviceType == typeof(Mapsui.Map))
            {
                return _map ??= new Mapsui.Map();
            }
            else if (serviceType == typeof(IDataSource))
            {
                return _dataSource ??= new DesignTimeDataSource();
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
                return _footprintDataSource ??= new FootprintDataSource();
            }
            else if (serviceType == typeof(TargetLayer))
            {
                return _groundTargetDataSource ??= new DesignTimeTargetLayer();
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
            public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GroundTracks => _groundTracks;
        }

        private class DesignTimeFootprintPreviewProvider : FootprintPreviewProvider
        {
            public DesignTimeFootprintPreviewProvider() : base()
            {
                AddSource(new DesignTimeFootprintPreviewSource());
            }

            private class DesignTimeFootprintPreviewSource : Data.Sources.IFootprintPreviewDataSource
            {
                public IEnumerable<FootprintPreview> GetFootprintPreviews()
                {
                    for (int i = 0; i < 8; i++)
                    {
                        yield return DesignTimeFootprintPreview.Build();
                    }
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
    }

    public class DesignDataGroundTargetProvider : GroundTargetProvider
    {
        public DesignDataGroundTargetProvider() : base()
        {
            AddSource(new DesignTimeGroundTargetDataSource());
        }

        private class DesignTimeGroundTargetDataSource : Data.Sources.IGroundTargetDataSource
        {
            public IEnumerable<GroundTarget> GetGroundTargets()
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return DesignTimeGroundTargetInfo.BuildModel();
                }
            }
        }
    }
}
