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
        private SatelliteProvider? _satelliteProvider;
        private FootprintLayer? _footprintDataSource;
        private TargetLayer? _groundTargetDataSource;
        private MapProvider? _mapProvider;
        private FootprintPreviewProvider? _footprintPreviewProvider;
        private FootprintPreviewGeometryProvider? _footprintPreviewGeometryProvider;
        private GroundTargetProvider? _groundTargetProvider;
        private UserGeometryProvider? _userGeometryProvider;
        private CustomProvider? _customProvider;
        private SatelliteViewer? _satelliteViewer;
        private FootprintObserver? _footprintObserver;
        private GroundTargetViewer? _groundTargetViewer;
        private SceneSearch? _sceneSearch;
        private MainViewModel? _mainViewModel;
        private SidePanel? _sidePanel;
        //private ViewModels.ToolBar? _toolBar;
        private CustomToolBar? _customToolBar;
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
            else if (serviceType == typeof(SatelliteProvider))
            {
                return _satelliteProvider ??= new DesignTimeSatelliteProvider();
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
            else if (serviceType == typeof(UserGeometryProvider))
            {
                return _userGeometryProvider ??= new DesignTimeUserGeometryProvider();
            }
            else if (serviceType == typeof(CustomProvider))
            {
                return _customProvider ??= new DesignTimeCustomProvider();
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
            else if (serviceType == typeof(CustomToolBar))
            {
                return _customToolBar ??= new CustomToolBar(this);
            }
            else if (serviceType == typeof(SidePanel))
            {
                return _sidePanel ??= new SidePanel();
            }
            else if (serviceType == typeof(FootprintLayer))
            {
                return _footprintDataSource ??= new DesignTimeFootprintLayer();
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

                public IEnumerable<Satellite> GetSatellites() => _satellites;

                public IDictionary<string, Dictionary<int, List<List<Point>>>> GetLeftStrips() => throw new Exception();

                public IDictionary<string, Dictionary<int, List<List<Point>>>> GetRightStrips() => throw new Exception();

                public IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> GetGroundTracks() => throw new Exception();
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
                public IEnumerable<FootprintPreview> GetFootprintPreviews()
                {
                    for (int i = 0; i < 8; i++)
                    {
                        yield return DesignTimeFootprintPreview.Build();
                    }
                }
            }
        }
        private class DesignTimeCustomProvider : CustomProvider
        {
            public DesignTimeCustomProvider() : base() { }
        }

        private class DesignTimeFootprintPreviewGeometryProvider : FootprintPreviewGeometryProvider
        {
            public DesignTimeFootprintPreviewGeometryProvider() : base() { }
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

                public IEnumerable<FootprintPreview> GetFootprintPreviews()
                {
                    for (int i = 0; i < 8; i++)
                    {
                        yield return DesignTimeFootprintPreview.Build();
                    }
                }

                public IEnumerable<UserGeometry> GetUserGeometries() 
                {
                    for (int i = 0; i < 10; i++)
                    {
                        yield return DesignTimeUserGeometryInfo.BuildModel();
                    }
                }              

                public void Remove(UserGeometry geometry) => throw new NotImplementedException();                
            }
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

        private class DesignTimeTargetLayer : Layers.TargetLayer
        {
            public DesignTimeTargetLayer() : base(new TargetLayerProvider(new DesignDataGroundTargetProvider()))
            {

            }
        }

        private class DesignTimeFootprintLayer : Layers.FootprintLayer
        {
            public DesignTimeFootprintLayer() : base(new FootprintLayerProvider(new DesignDataFootprintProvider()))
            {

            }
        }
    }

    internal class DesignDataGroundTargetProvider : GroundTargetProvider
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

    internal class DesignDataFootprintProvider : FootprintProvider
    {
        public DesignDataFootprintProvider() : base()
        {
            AddSource(new DesignTimeFootprintDataSource());
        }

        private class DesignTimeFootprintDataSource : Data.Sources.IFootprintDataSource
        {
            public IEnumerable<Footprint> GetFootprints()
            {
                for (int i = 0; i < 10; i++)
                {
                    yield return DesignTimeFootprintInfo.BuildModel();
                }
            }
        }
    }
}
