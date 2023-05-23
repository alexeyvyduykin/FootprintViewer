using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Layers.Providers;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Collections.Generic;
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

        var dataManager = Global.CreateDataManager();

        foreach (var item in config.MapBackgroundFiles)
        {
            dataManager.RegisterSource(DbKeys.Maps.ToString(), new FileSource(new[] { item }, MapResource.Build));
        }

        Global.AddLastPlannedSchedule(config, dataManager);

        var mapFactory = new MapFactory();

        var layerStyleManager = new LayerStyleManager();

        var featureManager = mapFactory.CreateFeatureManager();

        // StateMachines
        var mapState = new MapState();

        // Layer providers
        var groundTargetProvider = new GroundTargetProvider(dataManager, layerStyleManager);
        var trackProvider = new TrackProvider(dataManager, layerStyleManager);
        var sensorProvider = new SensorProvider(dataManager, layerStyleManager);
        var groundStationProvider = new GroundStationProvider(dataManager, layerStyleManager);
        var footprintProvider = new FootprintProvider(dataManager, layerStyleManager);
        var userGeometryProvider = new UserGeometryProvider(dataManager, layerStyleManager);

        Dictionary<LayerType, IProvider> providers = new()
        {
            { LayerType.GroundStation, groundStationProvider },
            { LayerType.GroundTarget, groundTargetProvider  },
            { LayerType.Sensor, sensorProvider  },
            { LayerType.Track, trackProvider },
            { LayerType.User, userGeometryProvider },
            { LayerType.Footprint, footprintProvider }
        };

        var map = mapFactory.CreateMap(layerStyleManager, providers);

        var mapNavigator = new MapNavigator((Map)map);

        var areaOfInterest = new AreaOfInterest((Map)map);

        serviceCollection.AddSingleton<IDataManager>(_ => dataManager);
        serviceCollection.AddSingleton<LayerStyleManager>(_ => layerStyleManager);
        serviceCollection.AddSingleton<FeatureManager>(_ => featureManager);
        serviceCollection.AddSingleton<Map>(_ => map);
        serviceCollection.AddSingleton<MapState>(_ => mapState);
        serviceCollection.AddSingleton<MapNavigator>(_ => mapNavigator);
        serviceCollection.AddSingleton<AreaOfInterest>(_ => areaOfInterest);

        serviceCollection.AddSingleton<FootprintProvider>(_ => footprintProvider);
        serviceCollection.AddSingleton<GroundStationProvider>(_ => groundStationProvider);
        serviceCollection.AddSingleton<GroundTargetProvider>(_ => groundTargetProvider);
        serviceCollection.AddSingleton<TrackProvider>(_ => trackProvider);
        serviceCollection.AddSingleton<SensorProvider>(_ => sensorProvider);
        serviceCollection.AddSingleton<UserGeometryProvider>(_ => userGeometryProvider);

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
