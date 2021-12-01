using FootprintViewer.Data;
using FootprintViewer.WPF;
using FootprintViewer.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
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

            var map = SampleBuilder.CreateMap();

            var userDataSource = new UserDataSource();

            var dataSource = CreateFromDatabase();

            map.SetWorldMapLayer(userDataSource.WorldMapSources.FirstOrDefault());

            var window = new MainWindow() { DataContext = new MainViewModel(map, userDataSource, dataSource) };

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
