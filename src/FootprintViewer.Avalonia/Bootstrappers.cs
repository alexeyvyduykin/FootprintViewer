using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Factories;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Localization;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using FootprintViewer.ViewModels.ToolBar;
using Mapsui;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Avalonia;

public static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, AppMode mode)
    {
        services.InitializeSplat();

        RegisterConfigurations(services, resolver);
        RegisterVariableViewModels(services, resolver, mode);
        RegisterViewModels(services, resolver);

        resolver.GetExistingService<TaskLoader>().Run();
    }

    private static void RegisterConfigurations(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var sourceConfig = new SourceBuilderConfiguration();
        configuration.GetSection("SourceBuilders").Bind(sourceConfig);
        services.RegisterConstant(sourceConfig, typeof(SourceBuilderConfiguration));

        var languagesConfig = new LanguagesConfiguration();
        configuration.GetSection("Languages").Bind(languagesConfig);
        services.RegisterConstant(languagesConfig, typeof(LanguagesConfiguration));
    }

    private static void RegisterVariableViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, AppMode mode)
    {
        switch (mode)
        {
            case AppMode.Release:
                {
                    services.Register<IDataFactory>(() => new ReleaseDataFactory());
                    break;
                }
            case AppMode.Demo:
                {
                    services.Register<IDataFactory>(() => new DemoDataFactory());
                    break;
                }
            case AppMode.DevWork:
                {
                    services.Register<IDataFactory>(() => new DevWorkDataFactory());
                    break;
                }
            case AppMode.DevHome:
                {
                    services.Register<IDataFactory>(() => new DevHomeDataFactory());
                    break;
                }
            default:
                services.Register<IDataFactory>(() => new ReleaseDataFactory());
                break;
        }
    }

    private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterConstant<TaskLoader>(new TaskLoader());
        services.Register(() => new ProjectFactory(resolver));
        services.Register(() => new MapFactory(resolver));

        var factory = resolver.GetExistingService<ProjectFactory>();
        var dataFactory = resolver.GetExistingService<IDataFactory>();
        var mapFactory = resolver.GetExistingService<MapFactory>();

        // LanguageManager
        services.RegisterConstant(factory.CreateLanguageManager(), typeof(ILanguageManager));

        // DataManager
        services.RegisterConstant(dataFactory.CreateDataManager(), typeof(IDataManager));

        services.RegisterConstant(new LayerStyleManager(), typeof(LayerStyleManager));

        services.RegisterConstant(mapFactory.CreateFeatureManager(), typeof(FeatureManager));

        // StateMachines
        services.RegisterConstant(new MapState(), typeof(MapState));

        // Layer providers
        services.RegisterConstant(new GroundTargetProvider(resolver), typeof(GroundTargetProvider));
        services.RegisterConstant(new TrackProvider(resolver), typeof(TrackProvider));
        services.RegisterConstant(new SensorProvider(resolver), typeof(SensorProvider));
        services.RegisterConstant(new GroundStationProvider(resolver), typeof(GroundStationProvider));
        services.RegisterConstant(new FootprintProvider(resolver), typeof(FootprintProvider));
        services.RegisterConstant(new UserGeometryProvider(resolver), typeof(UserGeometryProvider));

        services.RegisterConstant(mapFactory.CreateMap(), typeof(IMap));
        services.RegisterConstant(factory.CreateMapNavigator((Map)resolver.GetExistingService<IMap>()), typeof(IMapNavigator));
        services.RegisterConstant(new AreaOfInterest((Map)resolver.GetExistingService<IMap>()), typeof(AreaOfInterest));

        services.RegisterLazySingleton<SatelliteTabViewModel>(() => new SatelliteTabViewModel(resolver));
        services.RegisterLazySingleton<GroundTargetTabViewModel>(() => new GroundTargetTabViewModel(resolver));
        services.RegisterLazySingleton<FootprintTabViewModel>(() => new FootprintTabViewModel(resolver));
        services.RegisterLazySingleton<UserGeometryTabViewModel>(() => new UserGeometryTabViewModel(resolver));
        services.RegisterLazySingleton<GroundStationTabViewModel>(() => new GroundStationTabViewModel(resolver));
        services.RegisterLazySingleton<PlannedScheduleTabViewModel>(() => new PlannedScheduleTabViewModel(resolver));

        services.RegisterLazySingleton<ToolBarViewModel>(() => new ToolBarViewModel(resolver));

        services.RegisterLazySingleton<SidePanelViewModel>(() => new SidePanelViewModel()
        {
            Tabs = new List<SidePanelTabViewModel>(new SidePanelTabViewModel[]
            {
                resolver.GetExistingService<SatelliteTabViewModel>(),
                resolver.GetExistingService<GroundStationTabViewModel>(),
                resolver.GetExistingService<GroundTargetTabViewModel>(),
                resolver.GetExistingService<FootprintTabViewModel>(),
                resolver.GetExistingService<UserGeometryTabViewModel>(),
                resolver.GetExistingService<PlannedScheduleTabViewModel>(),
            })
        });

        services.RegisterLazySingleton<MainViewModel>(() => new MainViewModel(resolver));
    }
}
