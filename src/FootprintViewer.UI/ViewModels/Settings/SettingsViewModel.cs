using CliWrap;
using DynamicData;
using DynamicData.Alias;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Extensions;
using FootprintViewer.Logging;
using FootprintViewer.Services;
using FootprintViewer.Styles;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.Dialogs;
using FootprintViewer.UI.ViewModels.Settings.Items;
using FootprintViewer.UI.ViewModels.ToolBar;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.Settings;

public sealed class SettingsViewModel : DialogViewModelBase<object>
{
    private readonly SourceList<string> _mapBackgroundPaths = new();
    private readonly ReadOnlyObservableCollection<MapBackgroundItemViewModel> _items;
    private readonly List<string> _snapshotExtensions;
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public SettingsViewModel()
    {
        if (string.IsNullOrEmpty(Services.Config.FilePath) == false)
        {
            ConfigOnOpen = new Config(Services.Config.FilePath);
            ConfigOnOpen.LoadFile();
        }

        var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var mapService = Services.Locator.GetRequiredService<IMapService>();

        SnapshotDirectory = Services.MapSnapshotDir;

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // var mainState = Services.MainState;

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.0));

            //mainState?.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });

        _snapshotExtensions = AvailableExtensions();

        SelectedSnapshotExtension = ConfigOnOpen?.SelectedMapSnapshotExtension ?? _snapshotExtensions.First();

        var sources = localStorage.GetSources(DbKeys.Maps);

        var paths = sources
            .Where(s => s is FileSource)
            .Cast<FileSource>()
            .SelectMany(s => s.Paths)
            .ToList();

        _mapBackgroundPaths
            .Connect()
            .Select(s => new MapBackgroundItemViewModel(s))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _mapBackgroundPaths.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(paths);
        });

        AddMapBackground = ReactiveCommand.CreateFromTask(AddAsyncImpl);

        RemoveMapBackground = ReactiveCommand.Create<MapBackgroundItemViewModel>(RemoveImpl);

        OpenSnapshotDirectory = ReactiveCommand.CreateFromTask(OpenSnapshotDirectoryImpl);

        this.WhenAnyValue(s => s.SelectedSnapshotExtension)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Skip(1)
            .Subscribe(_ =>
            {
                Services.Config.SelectedMapSnapshotExtension = SelectedSnapshotExtension;
                Save();
            });

        _layers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => IsStyle(s, mapService.LayerStyle))
            .Transform(s => new LayerItemViewModel(s, mapService.LayerStyle))
            .Bind(out _layerItems)
            .Subscribe();

        Observable.StartAsync(() => UpdateLayersAsync(mapService.Map), RxApp.MainThreadScheduler);
    }

    private async Task UpdateLayersAsync(Map? map)
        => await Observable.Start(() => UpdateLayers(map), RxApp.TaskpoolScheduler);

    private static bool IsStyle(ILayer layer, LayerStyleManager styleManager)
    {
        return styleManager.GetStyle(layer.Name) is not null;
    }

    private void UpdateLayers(Map? map)
    {
        if (map is { })
        {
            _layers.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(map.Layers);
            });
        }
    }

    private async Task OpenSnapshotDirectoryImpl()
    {
        await Cli.Wrap("cmd").WithArguments($"/K start {SnapshotDirectory} && exit").ExecuteAsync();
        //await FileDialogHelper.ShowOpenFileDialogAsync("Snapshots", SnapshotExtensions.ToArray(), SnapshotDirectory);
    }

    private static List<string> AvailableExtensions()
    {
        return new[]
        {
            SKEncodedImageFormat.Png,
            SKEncodedImageFormat.Jpeg,
            SKEncodedImageFormat.Webp
        }
        .Select(ToStringExtension)
        .ToList();
    }

    private static string ToStringExtension(SKEncodedImageFormat type)
    {
        return $"*.{Enum.GetName(type)!.ToLower()}";
    }

    public static Config? ConfigOnOpen { get; set; }

    private static object ConfigLock { get; } = new();

    public override string Title { get => "Settings"; protected set { } }

    private void Save()
    {
        if (ConfigOnOpen is null)
        {
            return;
        }

        var config = new Config(ConfigOnOpen.FilePath);

        RxApp.MainThreadScheduler.Schedule(
            () =>
            {
                try
                {
                    lock (ConfigLock)
                    {
                        config.LoadFile();
                        EditConfigOnSave(config);
                        config.ToFile();

                        //OnConfigSaved();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex);
                }
            });
    }

    private void EditConfigOnSave(Config config)
    {
        var paths = MapBackgrounds
            .Where(s => !Equals(s.Name, "world"))
            .Select(s => s.FullPath);

        config.MapBackgroundFiles = paths.ToArray();

        config.SelectedMapSnapshotExtension = SelectedSnapshotExtension;
    }

    private async Task AddAsyncImpl()
    {
        try
        {
            var appService = Services.Locator.GetRequiredService<ApplicationService>();

            var filePath = await appService.ShowOpenFileDialogAsync("Add map background", new[] { "Mbtiles" });

            if (filePath is null)
            {
                return;
            }

            _mapBackgroundPaths.Edit(innerList =>
            {
                innerList.Add(filePath);
            });

            var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

            localStorage.RegisterSource(DbKeys.Maps.ToString(), new FileSource(new[] { filePath }, MapResource.Build));

            localStorage.UpdateData();

            Save();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private void RemoveImpl(MapBackgroundItemViewModel item)
    {
        _mapBackgroundPaths.Edit(innerList =>
        {
            innerList.Remove(item.FullPath);
        });

        var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        var source = localStorage
            .GetSources(DbKeys.Maps)
            .Where(s => s is FileSource)
            .Cast<FileSource>()
            .Where(s => s.Paths.Contains(item.FullPath))
            .Single();

        localStorage.UnregisterSource(DbKeys.Maps.ToString(), source);

        localStorage.UpdateData();

        Save();
    }

    public ReactiveCommand<Unit, Unit> OpenSnapshotDirectory { get; set; }

    public ReactiveCommand<Unit, Unit> AddMapBackground { get; set; }

    public ReactiveCommand<MapBackgroundItemViewModel, Unit> RemoveMapBackground { get; set; }

    public string SnapshotDirectory { get; set; }

    [Reactive]
    public string SelectedSnapshotExtension { get; set; }

    public List<string> SnapshotExtensions => _snapshotExtensions;

    public ReadOnlyObservableCollection<MapBackgroundItemViewModel> MapBackgrounds => _items;

    public IReadOnlyList<LayerItemViewModel> LayerItems => _layerItems;
}