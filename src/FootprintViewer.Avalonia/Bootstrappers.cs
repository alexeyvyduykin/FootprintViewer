using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.InitializeSplat();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.Register(() => new ProjectFactory(resolver));
            var factory = resolver.GetExistingService<ProjectFactory>();

            // Load the saved view model state.
            var settings = RxApp.SuspensionHost.GetAppState<AppSettings>();
            settings.Init(factory);
            services.RegisterConstant(settings, typeof(AppSettings));

            // Providers
            services.RegisterConstant(factory.CreateGroundStationProvider(), typeof(IProvider<GroundStationInfo>));

            // !!!
            //services.RegisterConstant(new Provider<GroundStationInfo>(), typeof(IProvider<GroundStationInfo>));

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
                resolver.GetExistingService<AppSettings>(),
            };

            services.RegisterConstant(new SidePanel() { Tabs = new List<SidePanelTab>(tabs) }, typeof(SidePanel));

            services.RegisterConstant(new MainViewModel(resolver), typeof(MainViewModel));
        }

    }
}
