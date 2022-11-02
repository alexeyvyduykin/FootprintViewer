using FootprintViewer.Configurations;
using FootprintViewer.Data;
using FootprintViewer.Data.Sources;
using FootprintViewer.Layers;
using FootprintViewer.Localization;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer;

public class ViewModelFactory
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;

    public ViewModelFactory(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
    }

    public SettingsTabViewModel CreateSettingsTabViewModel()
    {
        var configuration = _dependencyResolver.GetExistingService<SourceBuilderConfiguration>();

        var languageManager = _dependencyResolver.GetExistingService<ILanguageManager>();

        var userGeometryProvider = (EditableProvider<UserGeometry>)_dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();
        var footprintPreviewProvider = (Provider<FootprintPreview>)_dependencyResolver.GetExistingService<IProvider<FootprintPreview>>();
        var footprintPreviewGeometryProvider = (Provider<(string, NetTopologySuite.Geometries.Geometry)>)_dependencyResolver.GetExistingService<IProvider<(string, NetTopologySuite.Geometries.Geometry)>>();

        var userGeometryProviderViewModel = new ProviderViewModel(userGeometryProvider, _dependencyResolver)
        {
            Type = ProviderType.UserGeometries,
            AvailableBuilders = configuration.UserGeometrySourceBuilders,
            Builder = type => type switch
            {
                SourceType.Database => new DatabaseSourceViewModel(),
                SourceType.Random => new RandomSourceViewModel() { Name = "RandomUserGeometries" },
                _ => throw new Exception(),
            },
        };

        var footprintPreviewProviderViewModel = new ProviderViewModel(footprintPreviewProvider, _dependencyResolver)
        {
            Type = ProviderType.FootprintPreviews,
            AvailableBuilders = configuration.FootprintPreviewSourceBuilders,
            Builder = type => type switch
            {
                SourceType.Folder => new FolderSourceViewModel() { SearchPattern = "*.mbtiles" },
                _ => throw new Exception(),
            },
        };

        var footprintPreviewGeometryProviderViewModel = new ProviderViewModel(footprintPreviewGeometryProvider, _dependencyResolver)
        {
            Type = ProviderType.FootprintPreviewGeometries,
            AvailableBuilders = configuration.FootprintPreviewGeometrySourceBuilders,
            Builder = type => type switch
            {
                SourceType.File => new FileSourceViewModel() { FilterExtension = "*.mbtiles" },
                _ => throw new Exception(),
            },
        };

        var settingsViewer = new SettingsTabViewModel(_dependencyResolver)
        {
            Providers = new List<ProviderViewModel>()
            {
                userGeometryProviderViewModel,
                footprintPreviewGeometryProviderViewModel,
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
            SourceType.Random => new RandomSourceViewModel() { Name = "DefaultRandom" },
            _ => throw new Exception(),
        };
    }

    public IEnumerable<ISourceBuilderItem> CreateSourceBuilderItems(IEnumerable<string> builders, Func<SourceType, ISourceViewModel>? factory)
    {
        var list = new List<ISourceBuilderItem>();

        foreach (var item in builders)
        {
            if (Enum.TryParse(item.ToTitleCase(), out SourceType type) == true)
            {
                list.Add(new SourceBuilderItem(type, () => factory != null ? factory(type) : CreateSource(type)));
            }
        }

        return list;
    }

    public GroundStationTab CreateGroundStationTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new GroundStationTab(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public FootprintPreviewTab CreateFootprintPreviewTab()
    {
        var map = (Map)_dependencyResolver.GetExistingService<IMap>();
        var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();
        var provider = _dependencyResolver.GetExistingService<IProvider<FootprintPreview>>();

        var tab = new FootprintPreviewTab(_dependencyResolver);

        tab.SelectedItemObservable.Subscribe(footprint =>
        {
            if (footprint != null && footprint.Path != null)
            {
                var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                map.ReplaceLayer(layer, LayerType.FootprintImage);

                if (tab.Geometries.ContainsKey(footprint.Name!) == true)
                {
                    mapNavigator.SetFocusToPoint(tab.Geometries[footprint.Name!].Centroid.ToMPoint());
                }
            }
        });

        tab.Enter.Subscribe(footprint =>
        {
            if (tab.Geometries.ContainsKey(footprint.Name!) == true)
            {
                var layer = map.GetLayer(LayerType.FootprintImageBorder);

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.Add(new GeometryFeature() { Geometry = tab.Geometries[footprint.Name!] });
                    writableLayer.DataHasChanged();
                }
            }
        });

        tab.Leave.Subscribe(_ =>
        {
            var layer = map.GetLayer(LayerType.FootprintImageBorder);

            if (layer != null && layer is WritableLayer writableLayer)
            {
                writableLayer.Clear();
                writableLayer.DataHasChanged();
            }
        });

        var skip = provider.Sources.Count > 0 ? 1 : 0;

        provider.Observable
            .Skip(skip)
            .Select(s => Unit.Default)
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public FootprintTab CreateFootprintTab()
    {
        var mapNavigator = _dependencyResolver.GetExistingService<IMapNavigator>();
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new FootprintTab(_dependencyResolver);

        tab.Select.Select(s => s.Center).Subscribe(coord => mapNavigator.SetFocusToCoordinate(coord.X, coord.Y));

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public GroundTargetTab CreateGroundTargetTab()
    {
        var map = _dependencyResolver.GetExistingService<IMap>();
        var layer = map.GetLayer<Layer>(LayerType.GroundTarget);
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        var targetManager = layer?.BuildManager(() => ((TargetLayerSource)layer.DataSource!).GetFeatures());
        var tab = new GroundTargetTab(_dependencyResolver);

        tab.SelectedItemObservable.Subscribe(groundTarget =>
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    targetManager?.SelectFeature(name);
                }
            }
        });

        tab.Enter.Subscribe(groundTarget =>
        {
            var name = groundTarget.Name;

            if (name != null)
            {
                targetManager?.ShowHighlight(name);
            }
        });

        tab.Leave.Subscribe(_ =>
        {
            targetManager?.HideHighlight();
        });

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public SatelliteTab CreateSatelliteTab()
    {
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

        var tab = new SatelliteTab(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }

    public UserGeometryTab CreateUserGeometryTab()
    {
        var provider = _dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

        var tab = new UserGeometryTab(_dependencyResolver);

        var skip = provider.Sources.Count > 0 ? 1 : 0;

        provider.Observable
            .Skip(skip)
            .Select(s => Unit.Default)
            .InvokeCommand(tab.Loading);

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
                GenerateCount = randomSource.GenerateCount,
            };
        }
        else
        {
            throw new Exception();
        }
    }
}
