using CliWrap;
using DynamicData;
using DynamicData.Alias;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Extensions;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.Helpers;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using FootprintViewer.Fluent.ViewModels.Settings.Items;
using FootprintViewer.Fluent.ViewModels.ToolBar;
using FootprintViewer.Logging;
using FootprintViewer.Services;
using FootprintViewer.Styles;
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

namespace FootprintViewer.Fluent.ViewModels.Settings;

public sealed partial class SettingsViewModel : DialogViewModelBase<object>
{
    //  private readonly IDataManager _dataManager;
    private readonly SourceList<string> _mapBackgroundPaths = new();
    private readonly ReadOnlyObservableCollection<MapBackgroundItemViewModel> _items;
    private readonly List<string> _snapshotExtensions;
    private readonly LayerStyleManager _layerStyleManager;
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public SettingsViewModel()
    {
        ConfigOnOpen = new Config(Services.Config.FilePath);
        ConfigOnOpen.LoadFile();

        _layerStyleManager = Services.Locator.GetRequiredService<LayerStyleManager>();

        var map = Services.Locator.GetRequiredService<Map>();

        SnapshotDirectory = Services.MapSnapshotDir;

        // _dataManager = Services.Locator.GetRequiredService<IDataManager>();
        var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // var mainState = Services.MainState;

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            //mainState?.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });

        _snapshotExtensions = AvailableExtensions();

        SelectedSnapshotExtension = ConfigOnOpen.SelectedMapSnapshotExtension;

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
            .Where(s => IsStyle(s, _layerStyleManager))
            .Transform(s => new LayerItemViewModel(s, _layerStyleManager))
            .Bind(out _layerItems)
            .Subscribe();

        Observable.StartAsync(() => UpdateLayersAsync(map), RxApp.MainThreadScheduler);
    }

    private async Task UpdateLayersAsync(IMap? map) => await Observable.Start(() => UpdateLayers(map), RxApp.TaskpoolScheduler);

    private static bool IsStyle(ILayer layer, LayerStyleManager styleManager)
    {
        return styleManager.GetStyle(layer.Name) is not null;
    }

    private void UpdateLayers(IMap? map)
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
            var filePath = await FileDialogHelper.ShowOpenFileDialogAsync("Add map background", new[] { "mbtiles" });

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

            localStorage.UpdateData_Test_Remove_After();

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

        localStorage.UpdateData_Test_Remove_After();

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

public partial class SettingsViewModel
{
    public SettingsViewModel(DesignDataDependencyResolver resolver)
    {
        SnapshotDirectory = "C:\\Users\\User\\AppData\\Roaming\\FootprintViewer\\Client\\Snapshots";

        var localStorage = resolver.GetService<ILocalStorageService>();

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            Close(DialogResultKind.Normal);
        });

        _snapshotExtensions = AvailableExtensions();

        SelectedSnapshotExtension = _snapshotExtensions.First();

        var sources = localStorage.GetSources(DbKeys.Maps);

        var paths = sources
            .Where(s => s is FileSource)
            .Cast<FileSource>()
            .SelectMany(s => s.Paths).ToList();

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
            .Subscribe(_ => Save());

        _layerStyleManager = resolver.GetService<LayerStyleManager>();
        var map = resolver.GetService<IMap>();

        _layers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => IsStyle(s, _layerStyleManager))
            .Transform(s => new LayerItemViewModel(s, _layerStyleManager))
            .Bind(out _layerItems)
            .Subscribe();

        Observable.StartAsync(() => UpdateLayersAsync(map), RxApp.MainThreadScheduler);
    }
}