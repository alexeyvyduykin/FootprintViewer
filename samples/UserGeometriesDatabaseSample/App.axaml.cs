using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
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

        private static void RegisterSplat()
        {
            Locator.CurrentMutable.InitializeSplat();
            //Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.RegisterLazySingleton<IDataSource>(() => CreateDataSource());

            var locator = Locator.Current;

            Locator.CurrentMutable.RegisterLazySingleton<MainWindowViewModel>(() => new MainWindowViewModel(locator));

            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainWindowViewModel>));
            Locator.CurrentMutable.Register(() => new UserGeometryView(), typeof(IViewFor<UserGeometry>));
        }


        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                RegisterSplat();

                var mainViewModel = Locator.Current.GetService<MainWindowViewModel>();

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

        private static IDataSource CreateDataSource()
        {
            var db = new CustomDbContext(GetOptions());

            return new DataSource(db);
        }

        private static DbContextOptions<CustomDbContext> GetOptions()
        {
            string connectionString = "Host=localhost;Port=5432;Database=FootprintViewerDatabase;Username=postgres;Password=user";

            var optionsBuilder = new DbContextOptionsBuilder<CustomDbContext>();

            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(14, 1));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }
    }
}
