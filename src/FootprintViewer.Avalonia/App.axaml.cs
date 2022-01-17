#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Avalonia.Views;
using FootprintViewer.Data;
using FootprintViewer.Designer;
using FootprintViewer.ViewModels;
using Mapsui.Geometries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FootprintViewer.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            RegisterSplat();
        }

        static App()
        {
            InitializeDesigner();
        }

        public static void InitializeDesigner()
        {
            if (Design.IsDesignMode)
            {
                DesignerContext.InitializeContext();
            }
        }

        private void RegisterSplat()
        {
            Locator.CurrentMutable.InitializeSplat();

            Locator.CurrentMutable.RegisterLazySingleton<ProjectFactory>(() => new ProjectFactory());
            Locator.CurrentMutable.RegisterLazySingleton<IDataSource>(() => CreateFromDatabase());
            //Locator.CurrentMutable.RegisterLazySingleton<IDataSource>(() => CreateFromRandom());
            Locator.CurrentMutable.RegisterLazySingleton<UserDataSource>(() => new UserDataSource());

            var locator = Locator.Current;

            var map = locator.GetService<ProjectFactory>().CreateMap();

            Locator.CurrentMutable.RegisterLazySingleton<Mapsui.Map>(() => map);

            Locator.CurrentMutable.RegisterLazySingleton<SceneSearch>(() => new SceneSearch());
            Locator.CurrentMutable.RegisterLazySingleton<SatelliteViewer>(() => new SatelliteViewer());
            Locator.CurrentMutable.RegisterLazySingleton<GroundTargetViewer>(() => new GroundTargetViewer());
            Locator.CurrentMutable.RegisterLazySingleton<FootprintObserver>(() => new FootprintObserver());

            var tab1 = locator.GetService<SceneSearch>();
            var tab2 = locator.GetService<SatelliteViewer>();
            var tab3 = locator.GetService<GroundTargetViewer>();
            var tab4 = locator.GetService<FootprintObserver>();

            Locator.CurrentMutable.RegisterLazySingleton<SidePanel>(() => new SidePanel(new SidePanelTab[] { tab1, tab2, tab3, tab4 }));
            Locator.CurrentMutable.RegisterLazySingleton<MainViewModel>(() => new MainViewModel());
        }


        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                InitializationClassicDesktopStyle(desktopLifetime);

                var mainViewModel = Locator.Current.GetService<MainViewModel>();

                if (mainViewModel != null)
                {
                    desktopLifetime.MainWindow = new MainWindow()
                    {
                        DataContext = mainViewModel
                    };
                }
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                throw new Exception();
            }

            base.OnFrameworkInitializationCompleted();
        }

        public static void InitializationClassicDesktopStyle(IClassicDesktopStyleApplicationLifetime? desktopLifetime)
        {  
            var mainViewModel = Locator.Current.GetService<MainViewModel>();        
            var footprintObserver = Locator.Current.GetService<FootprintObserver>();
            var sceneSearch = Locator.Current.GetService<SceneSearch>();

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

        private static DbContextOptions<FootprintViewerDbContext> GetOptions()
        {
            var builder = new ConfigurationBuilder();
            // ��������� ���� � �������� ��������
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // �������� ������������ �� ����� appsettings.json
            builder.AddJsonFile("appsettings.json");
            // ������� ������������
            var config = builder.Build();
            // �������� ������ �����������
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
