using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Helpers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Models;
using FootprintViewer.Services;
using FootprintViewer.UI.Designer;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace FootprintViewer.UI;

public class App : Application
{
    private ApplicationStateManager? _applicationStateManager;

    static App()
    {
        InitializeDesigner();
    }

    public App()
    {
        Name = "FootprintViewer";
    }

    public static void InitializeDesigner()
    {
        if (Design.IsDesignMode == true)
        {
            Services.Locator.ConfigureServices(new DesignDataDependencyResolver());
        }
    }

    public static void ConfigureServices(Config config)
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        var localStorage = CreateLocalStorageService(config);
        var mapService = CreateMapService();

        serviceCollection.AddSingleton<ILocalStorageService>(_ => localStorage);
        serviceCollection.AddSingleton<IMapService>(_ => mapService);

        Services.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
    }

    public static ILocalStorageService CreateLocalStorageService(Config config)
    {
        var localStorage = new LocalStorageService();

        string embeddedFilePath = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mapSource = new FileSource(new[] { embeddedFilePath }, MapResource.Build);

        localStorage.RegisterSource(DbKeys.Maps.ToString(), mapSource);

        foreach (var item in config.MapBackgroundFiles)
        {
            localStorage.RegisterSource(DbKeys.Maps.ToString(), new FileSource(new[] { item }, MapResource.Build));
        }

        var sources = Global.GetSources(config);

        foreach (var (key, source) in sources)
        {
            localStorage.RegisterSource(key, source);
        }

        return localStorage;
    }

    public static IMapService CreateMapService()
    {
        var mapService = new MapService();

        mapService.AddLayerProvider(LayerType.GroundStation, new GroundStationProvider());
        mapService.AddLayerProvider(LayerType.GroundTarget, new GroundTargetProvider());
        mapService.AddLayerProvider(LayerType.Sensor, new SensorProvider());
        mapService.AddLayerProvider(LayerType.Track, new TrackProvider());
        mapService.AddLayerProvider(LayerType.Footprint, new FootprintProvider());
        mapService.AddLayerProvider(LayerType.User, new UserGeometryProvider());

        return mapService;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode == false)
        {

        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            _applicationStateManager = new ApplicationStateManager(desktopLifetime);

            DataContext = _applicationStateManager.ApplicationViewModel;

            RxApp.MainThreadScheduler.Schedule(
                async () =>
                {
                    MainViewModel.Instance.Initialize();

                    await MainViewModel.Instance.InitAsync();
                });
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            throw new Exception();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
