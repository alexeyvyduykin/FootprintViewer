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
using System.Reactive.Linq;
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

            services.RegisterLazySingleton<ProjectFactory>(() => new ProjectFactory(resolver));

            Data.Sources.IGroundTargetDataSource groundTargetDataSource;
            Data.Sources.IFootprintDataSource footprintDataSource;
            Data.Sources.ISatelliteDataSource satelliteDataSource;
            Data.Sources.IUserGeometryDataSource userGeometryDataSource;

            if (IsConnectionValid() == true)
            {
                var options = GetOptions();
                satelliteDataSource = new Data.Sources.SatelliteDataSource(options);
                groundTargetDataSource = new Data.Sources.GroundTargetDataSource(options);
                footprintDataSource = new Data.Sources.FootprintDataSource(options);
                userGeometryDataSource = new Data.Sources.UserGeometryDataSource(options);
            }
            else
            {
                satelliteDataSource = new Data.Sources.RandomSatelliteDataSource();
                footprintDataSource = new Data.Sources.RandomFootprintDataSource(satelliteDataSource);
                groundTargetDataSource = new Data.Sources.RandomGroundTargetDataSource(footprintDataSource);
                userGeometryDataSource = new Data.Sources.LocalUserGeometryDataSource();
            }

            // Satellites provider

            var satelliteProvider = new SatelliteProvider();
            satelliteProvider.AddSource(satelliteDataSource);
            services.RegisterLazySingleton<SatelliteProvider>(() => satelliteProvider);

            // Footprints provider

            var footprintProvider = new FootprintProvider();
            footprintProvider.AddSource(footprintDataSource);
            services.RegisterLazySingleton<FootprintProvider>(() => footprintProvider);
            var footprintLayerSource = new FootprintLayerSource(footprintProvider);
            services.RegisterLazySingleton<IFootprintLayerSource>(() => footprintLayerSource);

            // GroundTaregt provider

            var groundTargetProvider = new GroundTargetProvider();
            groundTargetProvider.AddSource(groundTargetDataSource);
            services.RegisterLazySingleton<GroundTargetProvider>(() => groundTargetProvider);

            // Map data provider

            var mapProvider = new MapProvider();
            mapProvider.AddSource(new Data.Sources.MapDataSource("*.mbtiles", "data", "world"));
            mapProvider.AddSource(new Data.Sources.MapDataSource("*.mbtiles", "userData", "world"));
            services.RegisterLazySingleton<MapProvider>(() => mapProvider);

            // Footprint preview provider

            var footprintPreviewProvider = new FootprintPreviewProvider();
            footprintPreviewProvider.AddSource(new Data.Sources.FootprintPreviewDataSource("*.mbtiles", "data", "footprints"));
            footprintPreviewProvider.AddSource(new Data.Sources.FootprintPreviewDataSource("*.mbtiles", "userData", "footprints"));
            services.RegisterLazySingleton<FootprintPreviewProvider>(() => footprintPreviewProvider);

            // Footprint preview geometry provider

            var footprintPreviewGeometryProvider = new FootprintPreviewGeometryProvider();
            footprintPreviewGeometryProvider.AddSource(new Data.Sources.FootprintPreviewGeometryDataSource("mosaic-tiff-ruonly.shp", "data", "mosaics-geotiff"));
            services.RegisterLazySingleton<FootprintPreviewGeometryProvider>(() => footprintPreviewGeometryProvider);

            // User geometry provider

            var userGeometryProvider = new UserGeometryProvider();
            userGeometryProvider.AddSource(userGeometryDataSource);
            services.RegisterLazySingleton<UserGeometryProvider>(() => userGeometryProvider);

            // Custom provider for user draw/edit

            CustomProvider customProvider = new CustomProvider(userGeometryProvider);
            services.RegisterLazySingleton<CustomProvider>(() => customProvider);

            // Layer style manager

            LayerStyleManager layerStyleManager = new LayerStyleManager();
            services.RegisterLazySingleton<LayerStyleManager>(() => layerStyleManager);


            var factory = resolver.GetExistingService<ProjectFactory>();

            var map = factory.CreateMap();

            services.RegisterLazySingleton<Mapsui.Map>(() => map);

            var targetLayer = map.GetLayer<TargetLayer>(LayerType.GroundTarget);

            services.RegisterLazySingleton<TargetLayer>(() => targetLayer);

            services.RegisterLazySingleton<SceneSearch>(() => new SceneSearch(resolver));
            services.RegisterLazySingleton<SatelliteViewer>(() => new SatelliteViewer(resolver));
            services.RegisterLazySingleton<GroundTargetViewer>(() => new GroundTargetViewer(resolver));
            services.RegisterLazySingleton<FootprintObserver>(() => new FootprintObserver(resolver));
            services.RegisterLazySingleton<UserGeometryViewer>(() => new UserGeometryViewer(resolver));

            services.RegisterLazySingleton<CustomToolBar>(() => new CustomToolBar(resolver));

            var tabs = new SidePanelTab[]
            {
                resolver.GetExistingService<SceneSearch>(),
                resolver.GetExistingService<SatelliteViewer>(),
                resolver.GetExistingService<GroundTargetViewer>(),
                resolver.GetExistingService<FootprintObserver>(),
                resolver.GetExistingService<UserGeometryViewer>(),
            };

            services.RegisterLazySingleton<SidePanel>(() => new SidePanel() { Tabs = new List<SidePanelTab>(tabs) });

            services.RegisterLazySingleton<MainViewModel>(() => new MainViewModel(resolver));
        }

        public void Initialization(IReadonlyDependencyResolver dependencyResolver)
        {
            Task.Run(async () => await LoadingAsync(dependencyResolver));
        }

        public async Task LoadingAsync(IReadonlyDependencyResolver dependencyResolver)
        {
            var userGeometryProvider = dependencyResolver.GetExistingService<UserGeometryProvider>();
            var footprintProvider = dependencyResolver.GetExistingService<FootprintProvider>();
            var satelliteProvider = dependencyResolver.GetExistingService<SatelliteProvider>();
            var groundTargetProvider = dependencyResolver.GetExistingService<GroundTargetProvider>();

            //await Task.Delay(TimeSpan.FromSeconds(4));

            await userGeometryProvider.Loading.Execute();

            await footprintProvider.Loading.Execute();

            await satelliteProvider.Loading.Execute();

            await groundTargetProvider.Loading.Execute();
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
        public static List<Window> AllWindows = new List<Window>();
    }
}
