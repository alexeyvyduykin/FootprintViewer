using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ReactiveUI;
using Splat;
using System;
using System.Text;
using UserGeometriesDatabaseSample.Data;
using UserGeometriesDatabaseSample.ViewModels;
using UserGeometriesDatabaseSample.Views;

namespace UserGeometriesDatabaseSample
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        static App()
        {
            Locator.CurrentMutable.InitializeSplat();
            //Locator.CurrentMutable.InitializeReactiveUI();
            RegisterBootstrapper(Locator.CurrentMutable, Locator.Current);
        }

        private static void RegisterBootstrapper(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            if (DatabaseExists() == true)
            {
                services.RegisterLazySingleton<IDataSource>(() => new DataSource(CreateDatabase()));
            }
            else
            {
                services.RegisterLazySingleton<IDataSource>(() => new LocalDataSource());
            }

            services.RegisterLazySingleton<MainWindowViewModel>(() => new MainWindowViewModel(resolver));

            services.Register(() => new MainWindow(), typeof(IViewFor<MainWindowViewModel>));
            services.Register(() => new UserGeometryView(), typeof(IViewFor<UserGeometry>));
        }

        private static T GetRequiredService<T>() => Locator.Current.GetRequiredService<T>();

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var mainViewModel = GetRequiredService<MainWindowViewModel>();

                desktopLifetime.MainWindow = new MainWindow()
                {
                    DataContext = mainViewModel
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime)
            {
                throw new Exception();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static CustomDbContext CreateDatabase()
        {
            return new CustomDbContext(GetOptions());
        }

        private static bool DatabaseExists()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
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

        private static string ConnectionString => "Host=localhost;Port=5432;Database=FootprintViewerDatabase;Username=postgres;Password=user";
        //private static string ConnectionString => "Host=654;Port=675;Database=gfdgfgdfgdfgdfgdf;Username=fghhgf;Password=fghhgf";

        private static DbContextOptions<CustomDbContext> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomDbContext>();

            var options = optionsBuilder.UseNpgsql(ConnectionString, options =>
            {
                options.SetPostgresVersion(new Version(14, 1));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }
    }
}
