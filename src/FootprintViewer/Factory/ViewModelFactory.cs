using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Splat;
using System;
using System.Collections.Generic;

namespace FootprintViewer
{
    public class ViewModelFactory
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;

        public ViewModelFactory(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IViewerList<SatelliteViewModel> CreateSatelliteViewerList(IProvider<Satellite> provider)
        {
            return new SatelliteViewerList(provider);
        }

        public IViewerList<FootprintViewModel> CreateFootprintViewerList(IProvider<Footprint> provider)
        {
            return new FootprintViewerList(provider);
        }

        public IViewerList<GroundTargetViewModel> CreateGroundTargetViewerList(IProvider<GroundTarget> provider)
        {
            return new GroundTargetViewerList(provider);
        }

        public IViewerList<GroundStationViewModel> CreateGroundStationViewerList(IProvider<GroundStation> provider)
        {
            return new GroundStationViewerList(provider);
        }

        public IViewerList<FootprintPreviewViewModel> CreateFootprintPreviewViewerList(IProvider<FootprintPreview> provider)
        {
            return new FootprintPreviewViewerList(provider);
        }

        public IViewerList<UserGeometryViewModel> CreateUserGeometryViewerList(IProvider<UserGeometry> provider)
        {
            return new UserGeometryViewerList(provider);
        }

        public SettingsTabViewModel CreateSettingsTabViewModel()
        {
            var configuration = _dependencyResolver.GetExistingService<SourceBuilderConfiguration>();
            var languagesConfiguration = _dependencyResolver.GetExistingService<LanguagesConfiguration>();

            var groundStationProvider = (Provider<GroundStation>)_dependencyResolver.GetExistingService<IProvider<GroundStation>>();
            var satelliteProvider = (Provider<Satellite>)_dependencyResolver.GetExistingService<IProvider<Satellite>>();
            var footprintProvider = (Provider<Footprint>)_dependencyResolver.GetExistingService<IProvider<Footprint>>();
            var groundTargetProvider = (Provider<GroundTarget>)_dependencyResolver.GetExistingService<IProvider<GroundTarget>>();
            var userGeometryProvider = (EditableProvider<UserGeometry>)_dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();
            var mapBackgroundProvider = (Provider<MapResource>)_dependencyResolver.GetExistingService<IProvider<MapResource>>();
            var footprintPreviewProvider = (Provider<FootprintPreview>)_dependencyResolver.GetExistingService<IProvider<FootprintPreview>>();
            var footprintPreviewGeometryProvider = (Provider<(string, NetTopologySuite.Geometries.Geometry)>)_dependencyResolver.GetExistingService<IProvider<(string, NetTopologySuite.Geometries.Geometry)>>();

            var groundStationProviderViewModel = new ProviderViewModel(groundStationProvider, _dependencyResolver)
            {
                Type = ProviderType.GroundStations,
                AvailableBuilders = configuration.GroundStationSourceBuilders,
            };

            var satelliteProviderViewModel = new ProviderViewModel(satelliteProvider, _dependencyResolver)
            {
                Type = ProviderType.Satellites,
                AvailableBuilders = configuration.SatelliteSourceBuilders,
            };

            var footprintProviderViewModel = new ProviderViewModel(footprintProvider, _dependencyResolver)
            {
                Type = ProviderType.Footprints,
                AvailableBuilders = configuration.FootprintSourceBuilders,
            };

            var groundTargetProviderViewModel = new ProviderViewModel(groundTargetProvider, _dependencyResolver)
            {
                Type = ProviderType.GroundTargets,
                AvailableBuilders = configuration.GroundTargetSourceBuilders,
            };

            var userGeometryProviderViewModel = new ProviderViewModel(userGeometryProvider, _dependencyResolver)
            {
                Type = ProviderType.UserGeometries,
                AvailableBuilders = configuration.UserGeometrySourceBuilders,
            };

            var mapBackgroundProviderViewModel = new ProviderViewModel(mapBackgroundProvider, _dependencyResolver)
            {
                Type = ProviderType.MapBackgrounds,
                AvailableBuilders = configuration.MapBackgroundSourceBuilders,
            };

            var footprintPreviewProviderViewModel = new ProviderViewModel(footprintPreviewProvider, _dependencyResolver)
            {
                Type = ProviderType.FootprintPreviews,
                AvailableBuilders = configuration.FootprintPreviewSourceBuilders,
            };

            var footprintPreviewGeometryProviderViewModel = new ProviderViewModel(footprintPreviewGeometryProvider, _dependencyResolver)
            {
                Type = ProviderType.FootprintPreviewGeometries,
                AvailableBuilders = configuration.FootprintPreviewGeometrySourceBuilders,
            };

            var languageManager = new LanguageManager(languagesConfiguration);
            
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
                },
                LanguageSettings = new LanguageSettingsViewModel(languageManager),
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

        public static IDataSource ToModel(this ISourceViewModel source)
        {
            if (source is IDatabaseSourceViewModel databaseSource)
            {
                return new DatabaseSource()
                {
                    Version = databaseSource.Version ?? string.Empty,
                    Host = databaseSource.Host ?? string.Empty,
                    Port = databaseSource.Port,
                    Database = databaseSource.Database ?? string.Empty,
                    Username = databaseSource.Username ?? string.Empty,
                    Password = databaseSource.Password ?? string.Empty,
                    Table = databaseSource.Table ?? string.Empty,
                };
            }
            else if (source is IFileSourceViewModel fileSource)
            {
                return new FileSource()
                {
                    Path = fileSource.Path ?? string.Empty,
                };
            }
            else if (source is IFolderSourceViewModel folderSource)
            {
                return new FolderSource()
                {
                    Directory = folderSource.Directory ?? string.Empty,
                    SearchPattern = folderSource.SearchPattern ?? string.Empty,
                };
            }
            else if (source is IRandomSourceViewModel randomSource)
            {
                return new RandomSource()
                {
                    Name = randomSource.Name ?? "RandomSourceDefault",
                };
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
