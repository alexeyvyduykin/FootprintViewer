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

        static App()
        {
            Locator.CurrentMutable.InitializeSplat();
            //Locator.CurrentMutable.InitializeReactiveUI();
            RegisterBootstrapper(Locator.CurrentMutable, Locator.Current);
        }

        private static void RegisterBootstrapper(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IDataSource>(() => CreateDataSource());

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
