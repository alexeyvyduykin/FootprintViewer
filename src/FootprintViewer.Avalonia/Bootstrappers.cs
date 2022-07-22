using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Avalonia
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, AppMode mode)
        {
            services.InitializeSplat();

            RegisterConfigurations(services, resolver);
            RegisterVariableViewModels(services, resolver, mode);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterConfigurations(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var sourceConfig = new SourceBuilderConfiguration();
            configuration.GetSection("SourceBuilders").Bind(sourceConfig);
            services.RegisterConstant(sourceConfig, typeof(SourceBuilderConfiguration));
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
            services.Register(() => new ProjectFactory(resolver));
            services.Register(() => new ViewModelFactory(resolver));
            services.Register(() => new MapFactory(resolver));

            var factory = resolver.GetExistingService<ProjectFactory>();
            var dataFactory = resolver.GetExistingService<IDataFactory>();
            var mapFactory = resolver.GetExistingService<MapFactory>();
            var viewModelFactory = resolver.GetExistingService<ViewModelFactory>();

            // Providers
            services.RegisterConstant(dataFactory.CreateGroundStationProvider(), typeof(IProvider<GroundStation>));
            services.RegisterConstant(dataFactory.CreateGroundTargetProvider(), typeof(IProvider<GroundTarget>));
            services.RegisterConstant(dataFactory.CreateFootprintProvider(), typeof(IProvider<Footprint>));
            services.RegisterConstant(dataFactory.CreateSatelliteProvider(), typeof(IProvider<Satellite>));
            services.RegisterConstant(dataFactory.CreateUserGeometryProvider(), typeof(IEditableProvider<UserGeometry>));
            services.RegisterConstant(dataFactory.CreateFootprintPreviewGeometryProvider(), typeof(IProvider<(string, NetTopologySuite.Geometries.Geometry)>));
            services.RegisterConstant(dataFactory.CreateMapBackgroundProvider(), typeof(IProvider<MapResource>));
            services.RegisterConstant(dataFactory.CreateFootprintPreviewProvider(), typeof(IProvider<FootprintPreview>));

            services.RegisterConstant(new LayerStyleManager(), typeof(LayerStyleManager));

            var satelliteProvider = resolver.GetExistingService<IProvider<Satellite>>();
            var footprintProvider = resolver.GetExistingService<IProvider<Footprint>>();
            var groundTargetProvider = resolver.GetExistingService<IProvider<GroundTarget>>();
            var groundStationProvider = resolver.GetExistingService<IProvider<GroundStation>>();
            var userGeometryProvider = resolver.GetExistingService<IEditableProvider<UserGeometry>>();

            services.RegisterConstant(new TrackLayerSource(satelliteProvider), typeof(ITrackLayerSource));
            services.RegisterConstant(new SensorLayerSource(satelliteProvider), typeof(ISensorLayerSource));
            services.RegisterConstant(new FootprintLayerSource(footprintProvider), typeof(IFootprintLayerSource));
            services.RegisterConstant(new TargetLayerSource(groundTargetProvider), typeof(ITargetLayerSource));
            services.RegisterConstant(new UserLayerSource(userGeometryProvider), typeof(IUserLayerSource));
            services.RegisterConstant(new GroundStationLayerSource(groundStationProvider), typeof(IGroundStationLayerSource));
            services.RegisterConstant(new EditLayerSource(), typeof(IEditLayerSource));

            services.RegisterConstant(mapFactory.CreateMap(), typeof(Mapsui.IMap));
            services.RegisterConstant(factory.CreateMapNavigator(), typeof(IMapNavigator));

            services.RegisterLazySingleton<SceneSearch>(() => factory.CreateSceneSearch());
            services.RegisterLazySingleton<SatelliteViewer>(() => factory.CreateSatelliteViewer());
            services.RegisterLazySingleton<GroundTargetTab>(() => factory.CreateGroundTargetTab());
            services.RegisterLazySingleton<FootprintTab>(() => factory.CreateFootprintTab());
            services.RegisterLazySingleton<UserGeometryViewer>(() => factory.CreateUserGeometryViewer());
            services.RegisterLazySingleton<GroundStationTab>(() => viewModelFactory.CreateGroundStationTab());
            services.RegisterLazySingleton<SettingsTabViewModel>(() => viewModelFactory.CreateSettingsTabViewModel());

            services.RegisterConstant(factory.CreateMapBackgroundList(), typeof(MapBackgroundList));

            services.RegisterConstant(new CustomToolBar(resolver), typeof(CustomToolBar));

            services.RegisterLazySingleton<SidePanel>(() => new SidePanel()
            {
                Tabs = new List<SidePanelTab>(new SidePanelTab[]
                {
                    resolver.GetExistingService<SceneSearch>(),
                    resolver.GetExistingService<SatelliteViewer>(),
                    resolver.GetExistingService<GroundStationTab>(),
                    resolver.GetExistingService<GroundTargetTab>(),
                    resolver.GetExistingService<FootprintTab>(),
                    resolver.GetExistingService<UserGeometryViewer>(),
                    resolver.GetExistingService<SettingsTabViewModel>(),
                })
            });

            services.RegisterLazySingleton<MainViewModel>(() => new MainViewModel(resolver));
        }
    }
}
