using FootprintViewer.Data.Managers;
using NetTopologySuite.Geometries;
using System;

namespace FootprintViewer.Data
{
    public abstract class BaseDataFactory : IDataFactory
    {
        private readonly RandomGroundStationDataManager _randomGroundStationDataManager;
        private readonly RandomSatelliteDataManager _randomSatelliteDataManager;
        private readonly RandomFootprintDataManager _randomFootprintDataManager;
        private readonly RandomGroundTargetDataManager _randomGroundTargetDataManager;

        protected BaseDataFactory()
        {
            _randomGroundStationDataManager = new RandomGroundStationDataManager();
            _randomSatelliteDataManager = new RandomSatelliteDataManager();
            _randomFootprintDataManager = new RandomFootprintDataManager(_randomSatelliteDataManager);
            _randomGroundTargetDataManager = new RandomGroundTargetDataManager(_randomFootprintDataManager);
        }

        public IProvider<GroundStation> CreateGroundStationProvider()
        {
            var provider = new Provider<GroundStation>();

            provider.AddSources(GetGroundStationSources());

            provider.AddManagers(new IDataManager<GroundStation>[]
            {
                new GroundStationDataManager(),
                _randomGroundStationDataManager,
            });

            //settings.WhenAnyValue(s => s.GroundStationProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<GroundTarget> CreateGroundTargetProvider()
        {
            var provider = new Provider<GroundTarget>();

            provider.AddSources(GetGroundTargetSources());

            provider.AddManagers(new IDataManager<GroundTarget>[]
            {
                new GroundTargetDataManager(),
                _randomGroundTargetDataManager,
            });

            //settings.WhenAnyValue(s => s.GroundTargetProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<Footprint> CreateFootprintProvider()
        {
            var provider = new Provider<Footprint>();

            provider.AddSources(GetFootprintSources());

            provider.AddManagers(new IDataManager<Footprint>[]
            {
                new FootprintDataManager(),
                _randomFootprintDataManager,
            });

            //settings.WhenAnyValue(s => s.FootprintProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<Satellite> CreateSatelliteProvider()
        {
            var provider = new Provider<Satellite>();

            provider.AddSources(GetSatelliteSources());

            provider.AddManagers(new IDataManager<Satellite>[]
            {
                new SatelliteDataManager(),
                _randomSatelliteDataManager,
            });

            //settings.WhenAnyValue(s => s.SatelliteProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IEditableProvider<UserGeometry> CreateUserGeometryProvider()
        {
            var provider = new EditableProvider<UserGeometry>();

            provider.AddSources(GetUserGeometrySources());

            provider.AddManagers(new IEditableDataManager<UserGeometry>[]
            {
                new UserGeometryDataManager(),
            });

            //settings.WhenAnyValue(s => s.UserGeometryProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<(string, Geometry)> CreateFootprintPreviewGeometryProvider()
        {
            var provider = new Provider<(string, Geometry)>();

            provider.AddSources(GetFootprintPreviewGeometrySources());

            provider.AddManagers(new IDataManager<(string, Geometry)>[]
            {
                new FootprintPreviewGeometryDataManager(),
            });

            //settings.WhenAnyValue(s => s.FootprintPreviewGeometryProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<MapResource> CreateMapBackgroundProvider()
        {
            var provider = new Provider<MapResource>();

            provider.AddSources(GetMapBackgroundSources());

            provider.AddManagers(new IDataManager<MapResource>[]
            {
                new MapDataManager(),
            });

            //settings.WhenAnyValue(s => s.MapBackgroundProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<FootprintPreview> CreateFootprintPreviewProvider()
        {
            var provider = new Provider<FootprintPreview>();

            provider.AddSources(GetFootprintPreviewSources());

            provider.AddManagers(new IDataManager<FootprintPreview>[]
            {
                new FootprintPreviewDataManager(),
            });

            //settings.WhenAnyValue(s => s.FootprintPreviewProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        //public ISourceBuilder[] CreateFootprintProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        new RandomSourceBuilder("RandomFootprints"),
        //        CreateDatabaseSourceBuilder(new TableInfo(){ Type = TableInfoType.Footprint }),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateGroundTargetProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        new RandomSourceBuilder("RandomGroundTargets"),
        //        CreateDatabaseSourceBuilder(new TableInfo(){ Type = TableInfoType.GroundTarget }),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateGroundStationProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        new RandomSourceBuilder("RandomGroundStations"),
        //        CreateDatabaseSourceBuilder(new TableInfo(){ Type = TableInfoType.GroundStation }),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateSatelliteProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        new RandomSourceBuilder("RandomSatellites"),
        //        CreateDatabaseSourceBuilder(new TableInfo(){ Type = TableInfoType.Satellite }),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateUserGeometryProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        CreateDatabaseSourceBuilder(new TableInfo(){ Type = TableInfoType.UserGeometry }),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateFootprintPreviewGeometryProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        CreateFileSourceBuilder("Shapefile", "shp"),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateMapBackgroundProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //       CreateFolderSourceBuilder("*.mbtiles"),
        //    };

        //    return builders;
        //}

        //public ISourceBuilder[] CreateFootprintPreviewProviderBuilders(ProviderSettings settings)
        //{
        //    var builders = new ISourceBuilder[]
        //    {
        //        CreateFolderSourceBuilder("*.mbtiles"),
        //    };

        //    return builders;
        //}

        //private enum TableType { Footprint, GroundTarget, Satellite, GroundStation, UserGeometry };

        //private ISourceBuilder CreateDatabaseSourceBuilder(TableInfo tableInfo)
        //{
        //    var builder = new DatabaseSourceBuilder(_dependencyResolver);

        //    builder.TableInfo = tableInfo;

        //    builder.Build.Subscribe(s =>
        //    {
        //        if (s is IDatabaseSourceInfo info)
        //        {
        //            var settings = _dependencyResolver.GetExistingService<AppSettingsState>();

        //            settings.LastDatabaseSource = info;
        //        }
        //    });

        //    return builder;
        //}

        //private ISourceBuilder CreateFolderSourceBuilder(string searchPattern)
        //{
        //    var builder = new FolderSourceBuilder(searchPattern);

        //    builder.Build.Subscribe(s =>
        //    {
        //        if (s is IFolderSourceInfo info)
        //        {
        //            var settings = _dependencyResolver.GetExistingService<AppSettingsState>();

        //            settings.LastOpenDirectory = info.Directory;
        //        }
        //    });

        //    return builder;
        //}

        //private ISourceBuilder CreateFileSourceBuilder(string fileName, string fileExtension)
        //{
        //    var builder = new FileSourceBuilder(fileName, fileExtension);

        //    builder.Build.Subscribe(s =>
        //    {
        //        if (s is IFileSourceInfo info)
        //        {
        //            var settings = _dependencyResolver.GetExistingService<AppSettingsState>();

        //            settings.LastOpenDirectory = System.IO.Path.GetDirectoryName(info.Path);
        //        }
        //    });

        //    return builder;
        //}

        protected virtual IDataSource[] GetFootprintPreviewSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetMapBackgroundSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetFootprintPreviewGeometrySources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetUserGeometrySources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetSatelliteSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetFootprintSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetGroundTargetSources()
        {
            return Array.Empty<IDataSource>();
        }

        protected virtual IDataSource[] GetGroundStationSources()
        {
            return Array.Empty<IDataSource>();
        }
    }
}
