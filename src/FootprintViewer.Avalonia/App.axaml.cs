using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Data;
using FootprintViewer.FileSystem;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private static void RegisterBootstrapper(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.InitializeSplat();

            services.Register(() => new ProjectFactory(resolver));

            var factory = resolver.GetExistingService<ProjectFactory>();

            IDataSource<GroundTargetInfo> groundTargetDataSource;
            IDataSource<FootprintInfo> footprintDataSource;
            IDataSource<SatelliteInfo> satelliteDataSource;
            IEditableDataSource<UserGeometryInfo> userGeometryDataSource;
            IDataSource<GroundStationInfo> groundStationDataSource;

            if (IsConnectionValid() == true)
            {
                var options = GetOptions();
                satelliteDataSource = new Data.Sources.SatelliteDataSource(options);
                groundTargetDataSource = new Data.Sources.GroundTargetDataSource(options);
                footprintDataSource = new Data.Sources.FootprintDataSource(options);
                userGeometryDataSource = new Data.Sources.UserGeometryDataSource(options);
                groundStationDataSource = new Data.Sources.GroundStationDataSource(options);
            }
            else
            {
                satelliteDataSource = new Data.Sources.RandomSatelliteDataSource();
                footprintDataSource = new Data.Sources.RandomFootprintDataSource(satelliteDataSource);
                groundTargetDataSource = new Data.Sources.RandomGroundTargetDataSource(footprintDataSource);
                userGeometryDataSource = new Data.Sources.LocalUserGeometryDataSource();
                groundStationDataSource = new Data.Sources.RandomGroundStationDataSource();
            }

            services.RegisterConstant(new Provider<SatelliteInfo>(new[] { satelliteDataSource }), typeof(IProvider<SatelliteInfo>));
            services.RegisterConstant(new Provider<FootprintInfo>(new[] { footprintDataSource }), typeof(IProvider<FootprintInfo>));
            services.RegisterConstant(new Provider<GroundTargetInfo>(new[] { groundTargetDataSource }), typeof(IProvider<GroundTargetInfo>));
            services.RegisterConstant(new Provider<MapResource>(new[]
            {
                new Data.Sources.MapDataSource("*.mbtiles", "data", "world"),
                new Data.Sources.MapDataSource("*.mbtiles", "userData", "world"),
            }), typeof(IProvider<MapResource>));
            services.RegisterConstant(new Provider<FootprintPreview>(new[]
            {
                new Data.Sources.FootprintPreviewDataSource("*.mbtiles", "data", "footprints"),
                new Data.Sources.FootprintPreviewDataSource("*.mbtiles", "userData", "footprints"),
            }), typeof(IProvider<FootprintPreview>));
            services.RegisterConstant(new Provider<(string, NetTopologySuite.Geometries.Geometry)>(new[]
            {
                new Data.Sources.FootprintPreviewGeometryDataSource("mosaic-tiff-ruonly.shp", "data", "mosaics-geotiff"),
            }), typeof(IProvider<(string, NetTopologySuite.Geometries.Geometry)>));
            services.RegisterConstant(new Provider<GroundStationInfo>(new[] { groundStationDataSource }), typeof(IProvider<GroundStationInfo>));
            services.RegisterConstant(new EditableProvider<UserGeometryInfo>(new[] { userGeometryDataSource }), typeof(IEditableProvider<UserGeometryInfo>));

            services.RegisterConstant(new LayerStyleManager(), typeof(LayerStyleManager));

            var satelliteProvider = resolver.GetExistingService<IProvider<SatelliteInfo>>();
            var footprintProvider = resolver.GetExistingService<IProvider<FootprintInfo>>();
            var groundTargetProvider = resolver.GetExistingService<IProvider<GroundTargetInfo>>();
            var groundStationProvider = resolver.GetExistingService<IProvider<GroundStationInfo>>();
            var userGeometryProvider = resolver.GetExistingService<IEditableProvider<UserGeometryInfo>>();

            services.RegisterConstant(new TrackLayerSource(satelliteProvider), typeof(ITrackLayerSource));
            services.RegisterConstant(new SensorLayerSource(satelliteProvider), typeof(ISensorLayerSource));
            services.RegisterConstant(new FootprintLayerSource(footprintProvider), typeof(IFootprintLayerSource));
            services.RegisterConstant(new TargetLayerSource(groundTargetProvider), typeof(ITargetLayerSource));
            services.RegisterConstant(new UserLayerSource(userGeometryProvider), typeof(IUserLayerSource));
            services.RegisterConstant(new GroundStationLayerSource(groundStationProvider), typeof(IGroundStationLayerSource));
            services.RegisterConstant(new EditLayerSource(), typeof(IEditLayerSource));

            services.RegisterConstant(factory.CreateMap(), typeof(Mapsui.IMap));
            services.RegisterConstant(factory.CreateMapNavigator(), typeof(IMapNavigator));

            services.RegisterConstant(new SceneSearch(resolver), typeof(SceneSearch));
            services.RegisterConstant(new SatelliteViewer(resolver), typeof(SatelliteViewer));
            services.RegisterConstant(new GroundTargetViewer(resolver), typeof(GroundTargetViewer));
            services.RegisterConstant(new FootprintObserver(resolver), typeof(FootprintObserver));
            services.RegisterConstant(new UserGeometryViewer(resolver), typeof(UserGeometryViewer));
            services.RegisterConstant(new GroundStationViewer(resolver), typeof(GroundStationViewer));

            services.RegisterConstant(new WorldMapSelector(resolver), typeof(WorldMapSelector));

            services.RegisterConstant(new CustomToolBar(resolver), typeof(CustomToolBar));

            var tabs = new SidePanelTab[]
            {
                resolver.GetExistingService<SceneSearch>(),
                resolver.GetExistingService<SatelliteViewer>(),
                resolver.GetExistingService<GroundStationViewer>(),
                resolver.GetExistingService<GroundTargetViewer>(),
                resolver.GetExistingService<FootprintObserver>(),
                resolver.GetExistingService<UserGeometryViewer>(),
            };

            services.RegisterConstant(new SidePanel() { Tabs = new List<SidePanelTab>(tabs) }, typeof(SidePanel));

            services.RegisterConstant(new MainViewModel(resolver), typeof(MainViewModel));
        }

        public static void Initialization(IReadonlyDependencyResolver dependencyResolver)
        {
            Task.Run(async () => await LoadingAsync(dependencyResolver));
        }

        public async static Task LoadingAsync(IReadonlyDependencyResolver dependencyResolver)
        {
            await Task.Run(() =>
            {
                var userGeometryProvider = dependencyResolver.GetExistingService<IEditableProvider<UserGeometryInfo>>();
                var footprintProvider = dependencyResolver.GetExistingService<IProvider<FootprintInfo>>();
                var satelliteProvider = dependencyResolver.GetExistingService<IProvider<SatelliteInfo>>();
                var groundTargetProvider = dependencyResolver.GetExistingService<IProvider<GroundTargetInfo>>();
                var groundStationProvider = dependencyResolver.GetExistingService<IProvider<GroundStationInfo>>();
                var mapProvider = dependencyResolver.GetExistingService<IProvider<MapResource>>();

                userGeometryProvider.Loading.Execute().Subscribe();

                footprintProvider.Loading.Execute().Subscribe();

                satelliteProvider.Loading.Execute().Subscribe();

                groundTargetProvider.Loading.Execute().Subscribe();

                groundStationProvider.Loading.Execute().Subscribe();

                mapProvider.Loading.Execute().Subscribe();
            });
        }

        private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                RegisterBootstrapper(Locator.CurrentMutable, Locator.Current);

                Initialization(Locator.Current);

                var mainViewModel = GetExistingService<MainViewModel>();

                if (mainViewModel != null)
                {
                    desktopLifetime.MainWindow = new Views.MainWindow()
                    {
                        DataContext = mainViewModel
                    };

                    WindowsManager.AllWindows.Add(desktopLifetime.MainWindow);
                }
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime)
            {
                throw new Exception();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static bool IsConnectionValid()
        {
            try
            {
                using (var connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            return builder.Build().GetConnectionString("DefaultConnection");
        }

        private static DbContextOptions<FootprintViewerDbContext> GetOptions()
        {
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(SolutionFolder.GetAppSettingsBasePath("appsettings.json"));
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");
            var major = int.Parse(config["PostgresVersionMajor"]);
            var minor = int.Parse(config["PostgresVersionMinor"]);

            var optionsBuilder = new DbContextOptionsBuilder<FootprintViewerDbContext>();
            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }

        public static Window? GetWindow()
        {
            if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }

            return null;
        }
    }

    public class WindowsManager
    {
        public static List<Window> AllWindows { get; } = new();
    }
}
