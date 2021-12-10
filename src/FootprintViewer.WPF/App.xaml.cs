using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using FootprintViewer.WPF;
using Mapsui.Geometries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace FootprintViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mapListener = new MapListener();

            var userDataSource = new UserDataSource();

            var dataSource = CreateFromDatabase();

            var map = ProjectFactory.CreateMap(dataSource);

            map.SetWorldMapLayer(userDataSource.WorldMapSources.FirstOrDefault());

            var sceneSearchTab = new SceneSearch()
            {
                Title = "Поиск сцены",
                Name = "Scene",
                Map = map,
                UserDataSource = userDataSource,
            };

            var satelliteViewerTab = new SatelliteViewer(map)
            {
                Title = "Просмотр спутников",
                Name = "SatelliteViewer",
                DataSource = dataSource,
            };

            var groundTargetViewerTab = new GroundTargetViewer(map)
            {
                Title = "Просмотр наземных целей",
                Name = "GroundTargetViewer",
            };

            var footprintObserverTab = new FootprintObserver(map)
            {
                Title = "Просмотр рабочей программы",
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

            mapListener.ClickOnMap += (s, e) => 
            {            
                if (s is string name && footprintObserverTab.IsActive == true)
                {
                    footprintObserverTab.SelectFootprintInfo(name);
                }
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

            var window = new MainWindow()
            {
                DataContext = mainViewModel
            };

            window.Show();
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
    }
}
