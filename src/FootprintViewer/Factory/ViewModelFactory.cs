using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

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

        public IViewerList<GroundStationViewModel> CreateGroundStationViewerList(IProvider<GroundStation> provider)
        {
            var viewerList = new GroundStationViewerList(provider);

            if (provider is Provider<GroundStation> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<GroundStationViewModel>?)null).InvokeCommand(viewerList.Loading);
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

            var groundStationProvider = (Provider<GroundStation>)_dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            var satelliteProvider = (Provider<Satellite>)_dependencyResolver.GetExistingService<IProvider<Satellite>>();
            var footprintProvider = (Provider<Footprint>)_dependencyResolver.GetExistingService<IProvider<Footprint>>();
            var groundTargetProvider = (Provider<GroundTarget>)_dependencyResolver.GetExistingService<IProvider<GroundTarget>>();
            var userGeometryProvider = (EditableProvider<UserGeometry>)_dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();
            var mapBackgroundProvider = (Provider<MapResource>)_dependencyResolver.GetExistingService<IProvider<MapResource>>();
            var footprintPreviewProvider = (Provider<FootprintPreview>)_dependencyResolver.GetExistingService<IProvider<FootprintPreview>>();
            var footprintPreviewGeometryProvider = (Provider<(string, NetTopologySuite.Geometries.Geometry)>)_dependencyResolver.GetExistingService<IProvider<(string, NetTopologySuite.Geometries.Geometry)>>();

            var groundStationProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.GroundStations,
                AvailableBuilders = configuration.GroundStationSourceBuilders,
            };

            var satelliteProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.Satellites,
                AvailableBuilders = configuration.SatelliteSourceBuilders,
            };

            var footprintProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.Footprints,
                AvailableBuilders = configuration.FootprintSourceBuilders,
            };

            var groundTargetProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.GroundTargets,
                AvailableBuilders = configuration.GroundTargetSourceBuilders,
            };

            var userGeometryProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.UserGeometries,
                AvailableBuilders = configuration.UserGeometrySourceBuilders,
            };

            var mapBackgroundProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.MapBackgrounds,
                AvailableBuilders = configuration.MapBackgroundSourceBuilders,
            };

            var footprintPreviewProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.FootprintPreviews,
                AvailableBuilders = configuration.FootprintPreviewSourceBuilders,
            };

            var footprintPreviewGeometryProviderViewModel = new ProviderViewModel(_dependencyResolver)
            {
                Type = ProviderType.FootprintPreviewGeometries,
                AvailableBuilders = configuration.FootprintPreviewGeometrySourceBuilders,
            };

            groundStationProviderViewModel.Sources.AddRange(groundStationProvider.GetSources().Select(s => s.ToViewModel()));
            satelliteProviderViewModel.Sources.AddRange(satelliteProvider.GetSources().Select(s => s.ToViewModel()));
            footprintProviderViewModel.Sources.AddRange(footprintProvider.GetSources().Select(s => s.ToViewModel()));
            groundTargetProviderViewModel.Sources.AddRange(groundTargetProvider.GetSources().Select(s => s.ToViewModel()));
            userGeometryProviderViewModel.Sources.AddRange(userGeometryProvider.GetSources().Select(s => s.ToViewModel()));
            mapBackgroundProviderViewModel.Sources.AddRange(mapBackgroundProvider.GetSources().Select(s => s.ToViewModel()));
            footprintPreviewProviderViewModel.Sources.AddRange(footprintPreviewProvider.GetSources().Select(s => s.ToViewModel()));
            footprintPreviewGeometryProviderViewModel.Sources.AddRange(footprintPreviewGeometryProvider.GetSources().Select(s => s.ToViewModel()));

            //groundStationProviderViewModel.AddSource.Subscribe(source => 
            //{
            //    groundStationProvider.AddSource();
            //});
            //groundStationProviderViewModel.RemoveSource.Subscribe();

            var settingsViewer = new SettingsTabViewModel(_dependencyResolver)
            {
                Providers = new List<ProviderViewModel>()
                {
                    footprintProviderViewModel,
                    groundTargetProviderViewModel,
                    groundStationProviderViewModel,
                    satelliteProviderViewModel,
                    userGeometryProviderViewModel,
                    footprintPreviewGeometryProviderViewModel,
                    mapBackgroundProviderViewModel,
                    footprintPreviewProviderViewModel,
                }
            };

            return settingsViewer;
        }

        private ISourceViewModel CreateSource(SourceType type)
        {
            return type switch
            {
                SourceType.File => new FileSourceViewModel(),
                SourceType.Folder => new FolderSourceViewModel(),
                SourceType.Database => new DatabaseSourceViewModel(),
                SourceType.Random => new RandomSourceViewModel("random"),
                _ => throw new Exception(),
            };
        }

        public IEnumerable<ISourceBuilderItem> CreateSourceBuilderItems(IEnumerable<string> builders)
        {
            var list = new List<ISourceBuilderItem>();

            foreach (var item in builders)
            {
                if (Enum.TryParse(item.ToTitleCase(), out SourceType type) == true)
                {
                    list.Add(new SourceBuilderItem(type, () => CreateSource(type)));
                }
            }

            return list;
        }

        public GroundStationTab CreateGroundStationTab()
        {
            var tab = new GroundStationTab(_dependencyResolver);

            return tab;
        }
    }

    public static class extns
    {
        public static ISourceViewModel ToViewModel(this IDataSource dataSource)
        {
            if (dataSource is IDatabaseSource databaseSource)
            {
                return new DatabaseSourceViewModel(databaseSource);
            }
            else if (dataSource is IFileSource fileSource)
            {
                return new FileSourceViewModel(fileSource);
            }
            else if (dataSource is IFolderSource folderSource)
            {
                return new FolderSourceViewModel(folderSource);
            }
            else if (dataSource is IRandomSource randomSource)
            {
                return new RandomSourceViewModel(randomSource);
            }
            else
            {
                throw new Exception();
            }
        }

        //public static IDataSource<T> ToModel<T>(this ISourceViewModel dataSource)
        //{
        //    if (dataSource is DatabaseSourceViewModel databaseSource)
        //    {
        //        return new DatabaseSourceViewModel(databaseSource);
        //    }
        //    else if (dataSource is IFileSource fileSource)
        //    {
        //        return new FileSourceViewModel(fileSource);
        //    }
        //    else if (dataSource is IFolderSource folderSource)
        //    {
        //        return new FolderSourceViewModel(folderSource);
        //    }
        //    else if (dataSource is IRandomSource randomSource)
        //    {
        //        return new RandomSourceViewModel(randomSource);
        //    }
        //    else
        //    {
        //        throw new Exception();
        //    }
        //}

    }
}
