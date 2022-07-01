using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;

namespace FootprintViewer
{
    public class ViewModelFactory
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;

        public ViewModelFactory(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IViewerList<SatelliteInfo> CreateSatelliteViewerList(IProvider<Satellite> provider)
        {
            var viewerList = new SatelliteViewerList(provider);

            if (provider is Provider<Satellite> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<SatelliteInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public IViewerList<FootprintInfo> CreateFootprintViewerList(IProvider<Footprint> provider)
        {
            var viewerList = new FootprintViewerList(provider);

            if (provider is Provider<Footprint> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<FootprintInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public IViewerList<GroundTargetInfo> CreateGroundTargetViewerList(IProvider<GroundTarget> provider)
        {
            var viewerList = new GroundTargetViewerList(provider);

            if (provider is Provider<GroundTarget> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<GroundTargetInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public IViewerList<GroundStationInfo> CreateGroundStationViewerList(IProvider<GroundStation> provider)
        {
            var viewerList = new GroundStationViewerList(provider);

            if (provider is Provider<GroundStation> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<GroundStationInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public IViewerList<FootprintPreviewInfo> CreateFootprintPreviewViewerList(IProvider<FootprintPreview> provider)
        {
            var viewerList = new FootprintPreviewViewerList(provider);

            if (provider is Provider<FootprintPreview> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<FootprintPreviewInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public IViewerList<UserGeometryInfo> CreateUserGeometryViewerList(IProvider<UserGeometry> provider)
        {
            var viewerList = new UserGeometryViewerList(provider);

            if (provider is EditableProvider<UserGeometry> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<UserGeometryInfo>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }

        public SettingsTabViewModel CreateSettingsTabViewModel()
        {
            var configuration = _dependencyResolver.GetExistingService<SourceBuilderConfiguration>();

            var providers = new List<ProviderViewModel>()
            {
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.Footprints,
                    AvailableBuilders = configuration.FootprintSourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.GroundTargets,
                    AvailableBuilders = configuration.GroundTargetSourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.GroundStations,
                    AvailableBuilders = configuration.GroundStationSourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.Satellites,
                    AvailableBuilders = configuration.SatelliteSourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.UserGeometries,
                    AvailableBuilders = configuration.UserGeometrySourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.FootprintPreviewGeometries,
                    AvailableBuilders = configuration.FootprintPreviewGeometrySourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.MapBackgrounds,
                    AvailableBuilders = configuration.MapBackgroundSourceBuilders,
                },
                new ProviderViewModel(_dependencyResolver)
                {
                    Type = ProviderType.FootprintPreviews,
                    AvailableBuilders = configuration.FootprintPreviewSourceBuilders,
                }
            };

            var settingsViewer = new SettingsTabViewModel(_dependencyResolver)
            {
                Providers = providers
            };

            return settingsViewer;
        }
    }
}
