using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.Services2;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Helpers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Services;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace FootprintViewer.Fluent;

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

        string embeddedFilePath = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");

        var mapSource = new FileSource(new[] { embeddedFilePath }, MapResource.Build);

        var localStorage = new LocalStorageService();

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

        var featureManager = new FeatureManager()
                .WithSelect(f => f[InteractiveFields.Select] = true)
                .WithUnselect(f => f[InteractiveFields.Select] = false)
                .WithEnter(f => f["Highlight"] = true)
                .WithLeave(f => f["Highlight"] = false);

        // StateMachines
        var mapState = new MapState();

        var mapService = new MapService();

        var areaOfInterest = new AreaOfInterest((Map)mapService.Map);

        mapService.AddLayerProvider(LayerType.GroundStation, new GroundStationProvider());
        mapService.AddLayerProvider(LayerType.GroundTarget, new GroundTargetProvider());
        mapService.AddLayerProvider(LayerType.Sensor, new SensorProvider());
        mapService.AddLayerProvider(LayerType.Track, new TrackProvider());
        mapService.AddLayerProvider(LayerType.Footprint, new FootprintProvider());
        mapService.AddLayerProvider(LayerType.User, new UserGeometryProvider());

        serviceCollection.AddSingleton<ILocalStorageService>(_ => localStorage);
        serviceCollection.AddSingleton<IMapService>(_ => mapService);
        serviceCollection.AddSingleton<FeatureManager>(_ => featureManager);
        serviceCollection.AddSingleton<MapState>(_ => mapState);
        serviceCollection.AddSingleton<AreaOfInterest>(_ => areaOfInterest);

        Services.Locator.ConfigureServices(serviceCollection.BuildServiceProvider());
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
