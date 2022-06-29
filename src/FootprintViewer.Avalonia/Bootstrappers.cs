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
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.InitializeSplat();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.Register(() => new ProjectFactory(resolver));
            services.Register(() => new ViewModelFactory(resolver));
            services.Register(() => new MapFactory(resolver));

            var factory = resolver.GetExistingService<ProjectFactory>();
            var mapFactory = resolver.GetExistingService<MapFactory>();

            // Load the saved view model state.
            var settings = RxApp.SuspensionHost.GetAppState<AppSettings>();
            settings.Init(factory);
            services.RegisterConstant(settings, typeof(AppSettings));

            // Providers
            services.RegisterConstant(factory.CreateGroundStationProvider(), typeof(IProvider<GroundStation>));
            services.RegisterConstant(factory.CreateGroundTargetProvider(), typeof(IProvider<GroundTarget>));
            services.RegisterConstant(factory.CreateFootprintProvider(), typeof(IProvider<Footprint>));
            services.RegisterConstant(factory.CreateSatelliteProvider(), typeof(IProvider<Satellite>));
            services.RegisterConstant(factory.CreateUserGeometryProvider(), typeof(IEditableProvider<UserGeometry>));
            services.RegisterConstant(factory.CreateFootprintPreviewGeometryProvider(), typeof(IProvider<(string, NetTopologySuite.Geometries.Geometry)>));
            services.RegisterConstant(factory.CreateMapBackgroundProvider(), typeof(IProvider<MapResource>));
            services.RegisterConstant(factory.CreateFootprintPreviewProvider(), typeof(IProvider<FootprintPreview>));

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
                resolver.GetExistingService<AppSettings>(),
            };

            services.RegisterConstant(new SidePanel() { Tabs = new List<SidePanelTab>(tabs) }, typeof(SidePanel));

            services.RegisterConstant(new MainViewModel(resolver), typeof(MainViewModel));
        }
    }
}
