using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Avalonia.Views;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;
using System;
using Mapsui.Geometries;
using System.Linq;
using Avalonia.Controls;
using System.Collections.Generic;

namespace FootprintViewer.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {

            var mapListener = new MapListener();

            var userDataSource = new UserDataSource();

            var dataSource = CreateFromDatabase();

            var map = ProjectFactory.CreateMap(dataSource);

            map.SetWorldMapLayer(userDataSource.WorldMapSources.FirstOrDefault());

            var sceneSearchTab = new SceneSearch()
            {
                Title = "����� �����",
                Name = "Scene",
                Map = map,
                UserDataSource = userDataSource,
            };

            var satelliteViewerTab = new SatelliteViewer(map)
            {
                Title = "�������� ���������",
                Name = "SatelliteViewer",
                DataSource = dataSource,
            };

            var groundTargetViewerTab = new GroundTargetViewer(map)
            {
                Title = "�������� �������� �����",
                Name = "GroundTargetViewer",
            };

            var footprintObserverTab = new FootprintObserver(map)
            {
                Title = "�������� ������� ���������",
                Name = "FootprintViewer",
                Filter = new FootprintObserverFilter(dataSource),
            };

            sceneSearchTab.Filter.FromDate = DateTime.Today.AddDays(-1);
            sceneSearchTab.Filter.ToDate = DateTime.Today.AddDays(1);

            var sidePanel = new SidePanel()
            {
                Tabs = new ObservableCollection<SidePanelTab>(new SidePanelTab[]
                {
                    sceneSearchTab,
                    satelliteViewerTab,
                    groundTargetViewerTab,
                    footprintObserverTab,
                }),

                SelectedTab = sceneSearchTab
            };

            var mainViewModel = new MainViewModel()
            {
                Map = map,
                MapListener = mapListener,
                UserDataSource = userDataSource,
                DataSource = dataSource,
                InfoPanel = ProjectFactory.CreateInfoPanel(),
                SidePanel = sidePanel,
            };

            mapListener.LeftClickOnMap += (s, e) =>
            {
                if (s is string name && footprintObserverTab.IsActive == true)
                {
                    if (mainViewModel.Plotter != null && (mainViewModel.Plotter.IsCreating == true || mainViewModel.Plotter.IsEditing == true))
                    {
                        return;
                    }

                    footprintObserverTab.SelectFootprintInfo(name);
                }
            };


            mainViewModel.AOIChanged += (s, e) =>
            {
                if (s != null)
                {
                    if (s is IGeometry geometry)
                    {
                        sceneSearchTab.SetAOI(geometry);
                    }
                }
                else
                {
                    sceneSearchTab.ResetAOI();
                }
            };

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {              
                desktop.MainWindow = new MainWindow() 
                {
                    DataContext = mainViewModel 
                };                
            }

            base.OnFrameworkInitializationCompleted();
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
