using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Avalonia.Views;
using FootprintViewer.Data;
using FootprintViewer.Designer;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
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

        static App()
        {
            InitializeDesigner();
        }

        public static void InitializeDesigner()
        {
            if (Design.IsDesignMode)
            {
                var designTimeData = new Designer.DesignTimeData();

                DesignerContext.InitializeContext(designTimeData);
            }
        }

        private static void RegisterBootstrapper(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.InitializeSplat();

            services.RegisterLazySingleton<ProjectFactory>(() => new ProjectFactory());
           
            if (IsConnectionValid() == true)
            {
                services.RegisterLazySingleton<IDataSource>(() => CreateFromDatabase());
            }
            else
            {
                services.RegisterLazySingleton<IDataSource>(() => CreateFromRandom());
            }

            services.RegisterLazySingleton<IUserDataSource>(() => new UserDataSource());

            var factory = resolver.GetExistingService<ProjectFactory>();

            var map = factory.CreateMap(resolver);

            services.RegisterLazySingleton<Mapsui.Map>(() => map);

            var footprintDataSource = (IFootprintDataSource)map.GetLayer<FootprintLayer>(LayerType.Footprint);
            var groundTargetDataSource = (IGroundTargetDataSource)map.GetLayer<TargetLayer>(LayerType.GroundTarget);

            services.RegisterLazySingleton<IFootprintDataSource>(() => footprintDataSource);
            services.RegisterLazySingleton<IGroundTargetDataSource>(() => groundTargetDataSource);

            services.RegisterLazySingleton<SceneSearch>(() => new SceneSearch(resolver));
            services.RegisterLazySingleton<SatelliteViewer>(() => new SatelliteViewer(resolver));
            services.RegisterLazySingleton<GroundTargetViewer>(() => new GroundTargetViewer(resolver));
            services.RegisterLazySingleton<FootprintObserver>(() => new FootprintObserver(resolver));

            Locator.CurrentMutable.RegisterLazySingleton<ToolBar>(() => new ToolBar(resolver));

            var tabs = new SidePanelTab[]
            {
                resolver.GetExistingService<SceneSearch>(),
                resolver.GetExistingService<SatelliteViewer>(),
                resolver.GetExistingService<GroundTargetViewer>(),
                resolver.GetExistingService<FootprintObserver>(),
            };

            Locator.CurrentMutable.RegisterLazySingleton<SidePanel>(() => new SidePanel() { Tabs = new List<SidePanelTab>(tabs) });

            Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel(resolver));
        }

        private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {                          
                RegisterBootstrapper(Locator.CurrentMutable, Locator.Current);

                InitializationClassicDesktopStyle();

                var mainViewModel = GetExistingService<MainViewModel>();
                
                if (mainViewModel != null)
                {
                    desktopLifetime.MainWindow = new MainWindow()
                    {
                        DataContext = mainViewModel
                    };
                }
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime)
            {
                throw new Exception();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static void InitializationClassicDesktopStyle()
        {
            var mainViewModel = Locator.Current.GetExistingService<MainViewModel>();
            var footprintObserver = Locator.Current.GetExistingService<FootprintObserver>();
            var sceneSearch = Locator.Current.GetExistingService<SceneSearch>();

            var mapListener = new MapListener();

            mapListener.LeftClickOnMap += (s, e) =>
            {
                if (s is string name && footprintObserver.IsActive == true)
                {
                    if (mainViewModel.Plotter != null && (mainViewModel.Plotter.IsCreating == true || mainViewModel.Plotter.IsEditing == true))
                    {
                        return;
                    }

                    footprintObserver.SelectFootprintInfo(name);
                }
            };

            mainViewModel.MapListener = mapListener;

            mainViewModel.AOIChanged += (s, e) =>
            {
                if (s != null)
                {
                    if (s is IGeometry geometry)
                    {
                        sceneSearch.SetAOI(geometry);
                    }
                }
                else
                {
                    sceneSearch.ResetAOI();
                }
            };
        }

        private static IDataSource CreateFromRandom()
        {
            return new LocalDataSource();
        }

        private static IDataSource CreateFromDatabase()
        {
            FootprintViewerDbContext db = new FootprintViewerDbContext(GetOptions());

            return new DatabaseDataSource(db);
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
            builder.SetBasePath(Directory.GetCurrentDirectory());
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
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
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
