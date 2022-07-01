using System;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Data
{
    public abstract class BaseDataFactory : IDataFactory
    {
        protected static string ToConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        protected static DbContextOptions<T> BuildDbContextOptions<T>(string version, string host, int port, string database, string username, string password) where T : DbCustomContext
        {
            var connectionString = ToConnectionString(host, port, database, username, password);
            var res = version!.Split(new[] { '.' });
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

        public IProvider<GroundStation> CreateGroundStationProvider()
        {
            var provider = new Provider<GroundStation>(GetGroundStationSources());

            //settings.WhenAnyValue(s => s.GroundStationProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<GroundTarget> CreateGroundTargetProvider()
        {
            var provider = new Provider<GroundTarget>(GetGroundTargetSources());

            //settings.WhenAnyValue(s => s.GroundTargetProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<Footprint> CreateFootprintProvider()
        {
            var provider = new Provider<Footprint>(GetFootprintSources());

            //settings.WhenAnyValue(s => s.FootprintProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<Satellite> CreateSatelliteProvider()
        {
            var provider = new Provider<Satellite>(GetSatelliteSources());

            //settings.WhenAnyValue(s => s.SatelliteProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IEditableProvider<UserGeometry> CreateUserGeometryProvider()
        {
            var provider = new EditableProvider<UserGeometry>(GetUserGeometrySources());

            //settings.WhenAnyValue(s => s.UserGeometryProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<(string, Geometry)> CreateFootprintPreviewGeometryProvider()
        {
            var provider = new Provider<(string, Geometry)>(GetFootprintPreviewGeometrySources());

            //settings.WhenAnyValue(s => s.FootprintPreviewGeometryProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<MapResource> CreateMapBackgroundProvider()
        {
            var provider = new Provider<MapResource>(GetMapBackgroundSources());

            //settings.WhenAnyValue(s => s.MapBackgroundProvider.Sources)
            //        .Skip(1)
            //        .Select(s => s.Select(s => ToDataSource(s)).ToArray())
            //        .Subscribe(provider.ChangeSources);

            return provider;
        }

        public IProvider<FootprintPreview> CreateFootprintPreviewProvider()
        {
            var provider = new Provider<FootprintPreview>(GetFootprintPreviewSources());

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

        protected virtual IDataSource<FootprintPreview>[] GetFootprintPreviewSources()
        {
            return Array.Empty<IDataSource<FootprintPreview>>();
        }

        protected virtual IDataSource<MapResource>[] GetMapBackgroundSources()
        {
            return Array.Empty<IDataSource<MapResource>>();
        }

        protected virtual IDataSource<(string, Geometry)>[] GetFootprintPreviewGeometrySources()
        {
            return Array.Empty<IDataSource<(string, Geometry)>>();
        }

        protected virtual IDataSource<UserGeometry>[] GetUserGeometrySources()
        {
            return Array.Empty<IDataSource<UserGeometry>>();
        }

        protected virtual IDataSource<Satellite>[] GetSatelliteSources()
        {
            return Array.Empty<IDataSource<Satellite>>();
        }

        protected virtual IDataSource<Footprint>[] GetFootprintSources()
        {
            return Array.Empty<IDataSource<Footprint>>();
        }

        protected virtual IDataSource<GroundTarget>[] GetGroundTargetSources()
        {
            return Array.Empty<IDataSource<GroundTarget>>();
        }

        protected virtual IDataSource<GroundStation>[] GetGroundStationSources()
        {
            return Array.Empty<IDataSource<GroundStation>>();
        }
    }
}
