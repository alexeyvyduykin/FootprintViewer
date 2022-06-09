using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.Settings;
using Npgsql;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
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

            // Providers
            services.RegisterConstant(factory.CreateGroundStationProvider(), typeof(IProvider<GroundStationInfo>));
            services.RegisterConstant(factory.CreateGroundTargetProvider(), typeof(IProvider<GroundTargetInfo>));
            services.RegisterConstant(factory.CreateFootprintProvider(), typeof(IProvider<FootprintInfo>));
            services.RegisterConstant(factory.CreateSatelliteProvider(), typeof(IProvider<SatelliteInfo>));
            services.RegisterConstant(factory.CreateUserGeometryProvider(), typeof(IEditableProvider<UserGeometryInfo>));
            services.RegisterConstant(factory.CreateFootprintPreviewGeometryProvider(), typeof(IProvider<(string, NetTopologySuite.Geometries.Geometry)>));
            services.RegisterConstant(factory.CreateMapBackgroundProvider(), typeof(IProvider<MapResource>));
            services.RegisterConstant(factory.CreateFootprintPreviewProvider(), typeof(IProvider<FootprintPreview>));

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

        private static bool IsConnectionValid(string? connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
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
