using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.Localization;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
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
        services.Register(() => new ViewModelFactory(resolver));
        services.Register(() => new MapFactory(resolver));

        var factory = resolver.GetExistingService<ProjectFactory>();
        var dataFactory = resolver.GetExistingService<IDataFactory>();
        var mapFactory = resolver.GetExistingService<MapFactory>();
        var viewModelFactory = resolver.GetExistingService<ViewModelFactory>();

        // LanguageManager
        services.RegisterConstant(factory.CreateLanguageManager(), typeof(ILanguageManager));

        // DataManager
        services.RegisterConstant(dataFactory.CreateDataManager(), typeof(IDataManager));

        services.RegisterConstant(new LayerStyleManager(), typeof(LayerStyleManager));

        services.RegisterConstant(new TargetLayerSource(), typeof(ITargetLayerSource));

        services.RegisterConstant(mapFactory.CreateMap(), typeof(Mapsui.IMap));
        services.RegisterConstant(factory.CreateMapNavigator(), typeof(IMapNavigator));

        services.RegisterLazySingleton<FootprintPreviewTabViewModel>(() => viewModelFactory.CreateFootprintPreviewTab());
        services.RegisterLazySingleton<SatelliteTabViewModel>(() => viewModelFactory.CreateSatelliteTab());
        services.RegisterLazySingleton<GroundTargetTabViewModel>(() => viewModelFactory.CreateGroundTargetTab());
        services.RegisterLazySingleton<FootprintTabViewModel>(() => viewModelFactory.CreateFootprintTab());
        services.RegisterLazySingleton<UserGeometryTabViewModel>(() => viewModelFactory.CreateUserGeometryTab());
        services.RegisterLazySingleton<GroundStationTabViewModel>(() => viewModelFactory.CreateGroundStationTab());
        services.RegisterLazySingleton<SettingsTabViewModel>(() => new SettingsTabViewModel(resolver));

        services.RegisterLazySingleton<CustomToolBar>(() => new CustomToolBar(resolver));

        services.RegisterLazySingleton<SidePanelViewModel>(() => new SidePanelViewModel()
        {
            Tabs = new List<SidePanelTabViewModel>(new SidePanelTabViewModel[]
            {
                resolver.GetExistingService<FootprintPreviewTabViewModel>(),
                resolver.GetExistingService<SatelliteTabViewModel>(),
                resolver.GetExistingService<GroundStationTabViewModel>(),
                resolver.GetExistingService<GroundTargetTabViewModel>(),
                resolver.GetExistingService<FootprintTabViewModel>(),
                resolver.GetExistingService<UserGeometryTabViewModel>(),
                resolver.GetExistingService<SettingsTabViewModel>(),
            })
        });

        services.RegisterLazySingleton<MainViewModel>(() => new MainViewModel(resolver));
    }
}
