using FootprintViewer.Configurations;
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

        var settingsViewer = new SettingsTabViewModel(_dependencyResolver)
        {
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
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

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

        dataManager.DataChanged
            .ToSignal()
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
        var dataManager = _dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        var tab = new UserGeometryTab(_dependencyResolver);

        dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(tab.Loading);

        return tab;
    }
}
