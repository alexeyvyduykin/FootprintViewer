using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using FootprintViewer.FileSystem;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer
{
    public class ProjectFactory
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;

        public ProjectFactory(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public Map CreateMap()
        {
            var map = new Map()
            {
                CRS = "EPSG:3857",
                //   Transformation = new MinimalTransformation(),
            };

            map.AddLayer(new Layer(), LayerType.WorldMap);
            map.AddLayer(new WritableLayer(), LayerType.FootprintImage);
            map.AddLayer(CreateGroundStationLayer(_dependencyResolver), LayerType.GroundStation);
            map.AddLayer(CreateTargetLayer(_dependencyResolver), LayerType.GroundTarget);
            map.AddLayer(CreateSensorLayer(_dependencyResolver), LayerType.Sensor);
            map.AddLayer(CreateTrackLayer(_dependencyResolver), LayerType.Track);
            map.AddLayer(CreateFootprintLayer(_dependencyResolver), LayerType.Footprint);
            map.AddLayer(CreateFootprintImageBorderLayer(_dependencyResolver), LayerType.FootprintImageBorder);
            map.AddLayer(CreateEditLayer(_dependencyResolver), LayerType.Edit);
            map.AddLayer(CreateVertexOnlyLayer(map, _dependencyResolver), LayerType.Vertex);
            map.AddLayer(CreateUserLayer(_dependencyResolver), LayerType.User);

            return map;
        }

        public IMapNavigator CreateMapNavigator()
        {
            return new MapNavigator();
        }

        private static ILayer CreateEditLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var source = dependencyResolver.GetExistingService<IEditLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new BaseCustomLayer<IEditLayerSource>(source)
            {
                Style = styleManager.EditStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateVertexOnlyLayer(Map map, IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var editLayer = map.GetLayer(LayerType.Edit);

            return new VertexOnlyLayer(editLayer!)
            {
                Style = styleManager.VertexOnlyStyle,
            };
        }

        private static ILayer CreateFootprintLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var source = dependencyResolver.GetExistingService<IFootprintLayerSource>();
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new FootprintLayer(source)
            {
                Style = styleManager.FootprintStyle,
                MaxVisiblePreview = styleManager.MaxVisibleFootprintStyle,
                IsMapInfoLayer = true,
            };
        }

        private static ILayer CreateGroundStationLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IGroundStationLayerSource>();

            return new BaseCustomLayer<IGroundStationLayerSource>(source)
            {
                Style = styleManager.GroundStationStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateTrackLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITrackLayerSource>();

            return new BaseCustomLayer<ITrackLayerSource>(source)
            {
                Style = styleManager.TrackStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateTargetLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ITargetLayerSource>();

            return new TargetLayer(source)
            {
                Style = styleManager.TargetStyle,
                MaxVisible = styleManager.MaxVisibleTargetStyle,
                IsMapInfoLayer = true,
            };
        }

        private static ILayer CreateSensorLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<ISensorLayerSource>();

            return new BaseCustomLayer<ISensorLayerSource>(source)
            {
                Style = styleManager.SensorStyle,
                IsMapInfoLayer = false,
            };
        }

        private static ILayer CreateUserLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();
            var source = dependencyResolver.GetExistingService<IUserLayerSource>();

            return new BaseCustomLayer<IUserLayerSource>(source)
            {
                IsMapInfoLayer = true,
                Style = styleManager.UserStyle,
            };
        }

        private static ILayer CreateFootprintImageBorderLayer(IReadonlyDependencyResolver dependencyResolver)
        {
            var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

            return new WritableLayer
            {
                Style = styleManager.FootprintImageBorderStyle,
            };
        }

        public InfoPanel CreateInfoPanel()
        {
            return new InfoPanel();
        }

        public BottomPanel CreateBottomPanel()
        {
            return new BottomPanel(_dependencyResolver);
        }

        public ScaleMapBar CreateScaleMapBar()
        {
            return new ScaleMapBar();
        }

        public MapBackgroundList CreateMapBackgroundList()
        {
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();

            var mapBackgroundList = new MapBackgroundList(_dependencyResolver);

            mapBackgroundList.Loading.Subscribe(s => { map.SetWorldMapLayer(s.First()); });

            mapBackgroundList.WorldMapChanged.Subscribe(s => map.SetWorldMapLayer(s));

            return mapBackgroundList;
        }

        public MapLayerList CreateMapLayerList()
        {
            return new MapLayerList(_dependencyResolver);
        }

        public SceneSearch CreateSceneSearch()
        {
            var map = (Map)_dependencyResolver.GetExistingService<IMap>();
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var sceneSearch = new SceneSearch(_dependencyResolver);

            sceneSearch.ViewerList.SelectedItemObservable.Subscribe(footprint =>
            {
                if (footprint != null && footprint.Path != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    map.ReplaceLayer(layer, LayerType.FootprintImage);

                    if (sceneSearch.Geometries.ContainsKey(footprint.Name!) == true)
                    {
                        mapNavigator.SetFocusToPoint(sceneSearch.Geometries[footprint.Name!].Centroid.ToMPoint());
                    }
                }
            });

            sceneSearch.ViewerList.MouseOverEnter.Subscribe(footprint =>
            {
                if (sceneSearch.Geometries.ContainsKey(footprint.Name!) == true)
                {
                    var layer = map.GetLayer(LayerType.FootprintImageBorder);

                    if (layer != null && layer is WritableLayer writableLayer)
                    {
                        writableLayer.Clear();
                        writableLayer.Add(new GeometryFeature() { Geometry = sceneSearch.Geometries[footprint.Name!] });
                        writableLayer.DataHasChanged();
                    }
                }
            });

            sceneSearch.ViewerList.MouseOverLeave.Subscribe(_ =>
            {
                var layer = map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.DataHasChanged();
                }
            });

            return sceneSearch;
        }

        public FootprintObserver CreateFootprintObserver()
        {
            var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();

            var footprintObserver = new FootprintObserver(_dependencyResolver);

            footprintObserver.ViewerList.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

            return footprintObserver;
        }

        public GroundTargetViewer CreateGroundTargetViewer()
        {
            var source = _dependencyResolver.GetExistingService<ITargetLayerSource>();

            var groundTargetViewer = new GroundTargetViewer(_dependencyResolver);

            groundTargetViewer.ViewerList.SelectedItemObservable.Subscribe(groundTarget =>
            {
                if (groundTarget != null)
                {
                    var name = groundTarget.Name;

                    if (string.IsNullOrEmpty(name) == false)
                    {
                        source.SelectFeature(name);
                    }
                }
            });

            groundTargetViewer.ViewerList.MouseOverEnter.Subscribe(groundTarget =>
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    source.ShowHighlight(name);
                }
            });

            groundTargetViewer.ViewerList.MouseOverLeave.Subscribe(_ =>
            {
                source.HideHighlight();
            });

            return groundTargetViewer;
        }

        public SatelliteViewer CreateSatelliteViewer()
        {
            var satelliteViewer = new SatelliteViewer(_dependencyResolver);

            return satelliteViewer;
        }

        public GroundStationViewer CreateGroundStationViewer()
        {
            var groundStationViewer = new GroundStationViewer(_dependencyResolver);

            return groundStationViewer;
        }

        public UserGeometryViewer CreateUserGeometryViewer()
        {
            var userGeometryViewer = new UserGeometryViewer(_dependencyResolver);

            return userGeometryViewer;
        }

        public IProvider<GroundStationInfo> CreateGroundStationProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.GroundStationProvider.Sources.Count == 0)
            {
                // settings.GroundStationSources.Add(new RandomSourceInfo("RandomGroundStation"));
                settings.GroundStationProvider.Sources.Add(new DatabaseSourceInfo()
                {
                    Version = "14.1",
                    Host = "localhost",
                    Port = 5432,
                    Database = "FootprintViewerDatabase",
                    Username = "postgres",
                    Password = "user",
                    Table = "GroundStations"
                });
            }

            var dataSources = settings.GroundStationProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<GroundStationInfo>(dataSources);

            settings.WhenAnyValue(s => s.GroundStationProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<GroundStationInfo> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo)
                {
                    throw new Exception();
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    return new GroundStationDataSource(DbOptions.Build<GroundStationDbContext>(databaseInfo));
                }
                else if (info is IRandomSourceInfo)
                {
                    return new RandomGroundStationDataSource();
                }

                throw new Exception();
            }
        }

        public IProvider<GroundTargetInfo> CreateGroundTargetProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.GroundTargetProvider.Sources.Count == 0)
            {
                // settings.GroundTargetSources.Add(new RandomSourceInfo("RandomGroundTarget"));
                settings.GroundTargetProvider.Sources.Add(new DatabaseSourceInfo()
                {
                    Version = "14.1",
                    Host = "localhost",
                    Port = 5432,
                    Database = "FootprintViewerDatabase",
                    Username = "postgres",
                    Password = "user",
                    Table = "GroundTargets"
                });
            }

            var dataSources = settings.GroundTargetProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<GroundTargetInfo>(dataSources);

            settings.WhenAnyValue(s => s.GroundTargetProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<GroundTargetInfo> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo)
                {
                    throw new Exception();
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    return new GroundTargetDataSource(DbOptions.Build<GroundTargetDbContext>(databaseInfo));
                }
                else if (info is IRandomSourceInfo)
                {
                    // TODO: temporary solution, all random sources not chaining
                    var satelliteDataSource = new RandomSatelliteDataSource();
                    var footprintDataSource = new RandomFootprintDataSource(satelliteDataSource);
                    return new RandomGroundTargetDataSource(footprintDataSource);
                }

                throw new Exception();
            }
        }

        public IProvider<FootprintInfo> CreateFootprintProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.FootprintProvider.Sources.Count == 0)
            {
                // settings.FootprintSources.Add(new RandomSourceInfo("RandomFootprint"));
                settings.FootprintProvider.Sources.Add(new DatabaseSourceInfo()
                {
                    Version = "14.1",
                    Host = "localhost",
                    Port = 5432,
                    Database = "FootprintViewerDatabase",
                    Username = "postgres",
                    Password = "user",
                    Table = "Footprints"
                });
            }

            var dataSources = settings.FootprintProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<FootprintInfo>(dataSources);

            settings.WhenAnyValue(s => s.FootprintProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<FootprintInfo> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo)
                {
                    throw new Exception();
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    return new FootprintDataSource(DbOptions.Build<FootprintDbContext>(databaseInfo));
                }
                else if (info is IRandomSourceInfo)
                {
                    // TODO: temporary solution, all random sources not chaining
                    var satelliteDataSource = new RandomSatelliteDataSource();
                    return new RandomFootprintDataSource(satelliteDataSource);
                }

                throw new Exception();
            }
        }

        public IProvider<SatelliteInfo> CreateSatelliteProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.SatelliteProvider.Sources.Count == 0)
            {
                // settings.SatelliteSources.Add(new RandomSourceInfo("RandomSatellite"));
                settings.SatelliteProvider.Sources.Add(new DatabaseSourceInfo()
                {
                    Version = "14.1",
                    Host = "localhost",
                    Port = 5432,
                    Database = "FootprintViewerDatabase",
                    Username = "postgres",
                    Password = "user",
                    Table = "Satellites"
                });
            }

            var dataSources = settings.SatelliteProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<SatelliteInfo>(dataSources);

            settings.WhenAnyValue(s => s.SatelliteProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<SatelliteInfo> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo)
                {
                    throw new Exception();
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    return new SatelliteDataSource(DbOptions.Build<SatelliteDbContext>(databaseInfo));
                }
                else if (info is IRandomSourceInfo)
                {
                    return new RandomSatelliteDataSource();
                }

                throw new Exception();
            }
        }

        public IEditableProvider<UserGeometryInfo> CreateUserGeometryProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.UserGeometryProvider.Sources.Count == 0)
            {
                settings.UserGeometryProvider.Sources.Add(new DatabaseSourceInfo()
                {
                    Version = "14.1",
                    Host = "localhost",
                    Port = 5432,
                    Database = "FootprintViewerDatabase",
                    Username = "postgres",
                    Password = "user",
                    Table = "UserGeometries"
                });
            }

            var dataSources = settings.UserGeometryProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new EditableProvider<UserGeometryInfo>(dataSources);

            settings.WhenAnyValue(s => s.UserGeometryProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<UserGeometryInfo> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo)
                {
                    throw new Exception();
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    return new UserGeometryDataSource(DbOptions.Build<UserGeometryDbContext>(databaseInfo));
                }
                else if (info is IRandomSourceInfo)
                {
                    throw new Exception();
                }

                throw new Exception();
            }
        }

        public IProvider<(string, NetTopologySuite.Geometries.Geometry)> CreateFootprintPreviewGeometryProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.FootprintPreviewGeometryProvider.Sources.Count == 0)
            {
                var folder = new SolutionFolder("data");
                var path = folder.GetPath("mosaic-tiff-ruonly.shp", "mosaics-geotiff");

                settings.FootprintPreviewGeometryProvider.Sources.Add(new FileSourceInfo()
                {
                    Path = path,
                    FilterName = "Shapefile",
                    FilterExtension = "shp",
                });
            }

            var dataSources = settings.FootprintPreviewGeometryProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<(string, NetTopologySuite.Geometries.Geometry)>(dataSources);

            settings.WhenAnyValue(s => s.FootprintPreviewGeometryProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<(string, NetTopologySuite.Geometries.Geometry)> ToDataSource(ISourceInfo info)
            {
                if (info is IFileSourceInfo fileInfo)
                {
                    return new FootprintPreviewGeometryDataSource(fileInfo.Path);
                }
                else if (info is IDatabaseSourceInfo databaseInfo)
                {
                    throw new Exception();
                }
                else if (info is IRandomSourceInfo)
                {
                    throw new Exception();
                }

                throw new Exception();
            }
        }

        public IProvider<MapResource> CreateMapBackgroundProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.MapBackgroundProvider.Sources.Count == 0)
            {
                var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "world");
                var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "world");

                settings.MapBackgroundProvider.Sources.Add(new FolderSourceInfo()
                {
                    Directory = directory1,
                    SearchPattern = "*.mbtiles",
                });

                settings.MapBackgroundProvider.Sources.Add(new FolderSourceInfo()
                {
                    Directory = directory2,
                    SearchPattern = "*.mbtiles",
                });
            }

            var dataSources = settings.MapBackgroundProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<MapResource>(dataSources);

            settings.WhenAnyValue(s => s.MapBackgroundProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);

            return provider;

            static IDataSource<MapResource> ToDataSource(ISourceInfo info)
            {
                if (info is IFolderSourceInfo folderInfo)
                {
                    return new MapDataSource(folderInfo.Directory, folderInfo.SearchPattern);
                }

                throw new Exception();
            }
        }

        public IProvider<FootprintPreview> CreateFootprintPreviewProvider()
        {
            var settings = _dependencyResolver.GetService<AppSettings>()!;

            if (settings.FootprintPreviewProvider.Sources.Count == 0)
            {
                var directory1 = System.IO.Path.Combine(new SolutionFolder("data").FolderDirectory, "footprints");
                var directory2 = System.IO.Path.Combine(new SolutionFolder("userData").FolderDirectory, "footprints");

                settings.FootprintPreviewProvider.Sources.Add(new FolderSourceInfo()
                {
                    Directory = directory1,
                    SearchPattern = "*.mbtiles",
                });

                settings.FootprintPreviewProvider.Sources.Add(new FolderSourceInfo()
                {
                    Directory = directory2,
                    SearchPattern = "*.mbtiles",
                });
            }

            var dataSources = settings.FootprintPreviewProvider.Sources.Select(s => ToDataSource(s)).ToArray();

            var provider = new Provider<FootprintPreview>(dataSources);

            settings.WhenAnyValue(s => s.FootprintPreviewProvider.Sources)
                    .Skip(1)
                    .Select(s => s.Select(s => ToDataSource(s)).ToArray())
                    .Subscribe(provider.ChangeSources);



            return provider;

            static IDataSource<FootprintPreview> ToDataSource(ISourceInfo info)
            {
                if (info is IFolderSourceInfo folderInfo)
                {
                    return new FootprintPreviewDataSource(folderInfo.Directory, folderInfo.SearchPattern);
                }

                throw new Exception();
            }
        }

        public ISourceBuilder[] CreateFootprintProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                new RandomSourceBuilder("RandomFootprints"),
                CreateDatabaseSourceBuilder(),
            };

            foreach (var item in builders)
            {
                item.Build
                    .Where(s => s != null)
                    .Select(s => s!)
                    .InvokeCommand(
                    settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateGroundTargetProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                new RandomSourceBuilder("RandomGroundTargets"),
                CreateDatabaseSourceBuilder(),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateGroundStationProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                new RandomSourceBuilder("RandomGroundStations"),
                CreateDatabaseSourceBuilder(),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateSatelliteProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                new RandomSourceBuilder("RandomSatellites"),
                CreateDatabaseSourceBuilder(),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateUserGeometryProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                CreateDatabaseSourceBuilder(),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateFootprintPreviewGeometryProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                CreateFileSourceBuilder("Shapefile", "shp"),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateMapBackgroundProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
               CreateFolderSourceBuilder("*.mbtiles"),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        public ISourceBuilder[] CreateFootprintPreviewProviderBuilders(ProviderSettings settings)
        {
            var builders = new ISourceBuilder[]
            {
                CreateFolderSourceBuilder("*.mbtiles"),
            };

            foreach (var item in builders)
            {
                item.Build.Where(s => s != null).Select(s => s!).InvokeCommand(settings.AddSource);
            }

            return builders;
        }

        private ISourceBuilder CreateDatabaseSourceBuilder()
        {
            var builder = new DatabaseSourceBuilder(_dependencyResolver);

            builder.Build.Subscribe(s =>
            {
                if (s is IDatabaseSourceInfo info)
                {
                    var settings = _dependencyResolver.GetExistingService<AppSettings>();

                    settings.LastDatabaseSource = info;
                }
            });

            return builder;
        }

        private ISourceBuilder CreateFolderSourceBuilder(string searchPattern)
        {
            var builder = new FolderSourceBuilder(searchPattern);

            builder.Build.Subscribe(s =>
            {
                if (s is IFolderSourceInfo info)
                {
                    var settings = _dependencyResolver.GetExistingService<AppSettings>();

                    settings.LastOpenDirectory = info.Directory;
                }
            });

            return builder;
        }

        private ISourceBuilder CreateFileSourceBuilder(string fileName, string fileExtension)
        {
            var builder = new FileSourceBuilder(fileName, fileExtension);

            builder.Build.Subscribe(s =>
            {
                if (s is IFileSourceInfo info)
                {
                    var settings = _dependencyResolver.GetExistingService<AppSettings>();

                    settings.LastOpenDirectory = System.IO.Path.GetDirectoryName(info.Path);
                }
            });

            return builder;
        }
    }

    public static class DbOptions
    {
        public static string BuildConnectionString(IDatabaseSourceInfo info)
        {
            return $"Host={info.Host};Port={info.Port};Database={info.Database};Username={info.Username};Password={info.Password}";
        }

        public static DbContextOptions<T> Build<T>(IDatabaseSourceInfo info) where T : DbContext
        {
            var connectionString = BuildConnectionString(info);
            var res = info.Version!.Split(new[] { '.' });
            var major = int.Parse(res[0]);
            var minor = int.Parse(res[1]);

            var optionsBuilder = new DbContextOptionsBuilder<T>();

            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }
    }
}
