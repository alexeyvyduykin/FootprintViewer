using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.Data;
using FootprintViewer.FileSystem;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

            // Load the saved view model state.
            var settings = RxApp.SuspensionHost.GetAppState<AppSettings>();

            services.RegisterConstant(settings, typeof(AppSettings));

            services.Register(() => new ProjectFactory(resolver));

            var factory = resolver.GetExistingService<ProjectFactory>();

            IDataSource<FootprintInfo> footprintDataSource;
            IDataSource<SatelliteInfo> satelliteDataSource;
            IEditableDataSource<UserGeometryInfo> userGeometryDataSource;

            if (IsConnectionValid() == true)
            {
                var options = GetOptions();
                satelliteDataSource = new Data.Sources.SatelliteDataSource(options);
                footprintDataSource = new Data.Sources.FootprintDataSource(options);
                userGeometryDataSource = new Data.Sources.UserGeometryDataSource(options);
            }
            else
            {
                satelliteDataSource = new Data.Sources.RandomSatelliteDataSource();
                footprintDataSource = new Data.Sources.RandomFootprintDataSource(satelliteDataSource);
                userGeometryDataSource = new Data.Sources.LocalUserGeometryDataSource();
            }

            services.RegisterConstant(new Provider<SatelliteInfo>(new[] { satelliteDataSource }), typeof(IProvider<SatelliteInfo>));
            services.RegisterConstant(new Provider<FootprintInfo>(new[] { footprintDataSource }), typeof(IProvider<FootprintInfo>));
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

            // Providers
            services.RegisterConstant(factory.CreateGroundStationProvider(), typeof(IProvider<GroundStationInfo>));
            services.RegisterConstant(factory.CreateGroundTargetProvider(), typeof(IProvider<GroundTargetInfo>));

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

            services.RegisterConstant(factory.CreateSceneSearch(), typeof(SceneSearch));
            services.RegisterConstant(factory.CreateSatelliteViewer(), typeof(SatelliteViewer));
            services.RegisterConstant(factory.CreateGroundTargetViewer(), typeof(GroundTargetViewer));
            services.RegisterConstant(factory.CreateFootprintObserver(), typeof(FootprintObserver));
            services.RegisterConstant(factory.CreateUserGeometryViewer(), typeof(UserGeometryViewer));
            services.RegisterConstant(factory.CreateGroundStationViewer(), typeof(GroundStationViewer));

            services.RegisterConstant(factory.CreateMapBackgroundList(), typeof(MapBackgroundList));

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

        private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Create the AutoSuspendHelper.
            var suspension = new AutoSuspendHelper(ApplicationLifetime!);
            RxApp.SuspensionHost.CreateNewAppState = () => new AppSettings();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new Drivers.NewtonsoftJsonSuspensionDriver("_appsettings.json"));
            suspension.OnFrameworkInitializationCompleted();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                RegisterBootstrapper(Locator.CurrentMutable, Locator.Current);

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
