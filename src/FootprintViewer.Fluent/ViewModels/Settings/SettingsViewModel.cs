using CliWrap;
using DynamicData;
using DynamicData.Alias;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Data.Sources;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.Helpers;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using FootprintViewer.Fluent.ViewModels.Settings.Items;
using FootprintViewer.Logging;
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
    private readonly IDataManager _dataManager;
    private readonly SourceList<string> _mapBackgroundPaths = new();
    private readonly ReadOnlyObservableCollection<MapBackgroundItemViewModel> _items;
    private readonly List<string> _snapshotExtensions;

    public SettingsViewModel()
    {
        ConfigOnOpen = new Config(Services.Config.FilePath);
        ConfigOnOpen.LoadFile();

        SnapshotDirectory = Services.MapSnapshotDir;

        _dataManager = Services.DataManager;

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var mainState = Services.MainState;

            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            //mainState?.SaveData(_dataManager);

            Close(DialogResultKind.Normal);
        });

        _snapshotExtensions = AvailableExtensions();

        SelectedSnapshotExtension = ConfigOnOpen.SelectedMapSnapshotExtension;

        var sources = _dataManager.GetSources(DbKeys.Maps);

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

            Services.DataManager.RegisterSource(DbKeys.Maps.ToString(), new FileSource(new[] { filePath }, MapResource.Build));

            Services.DataManager.UpdateData();

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

        var source = Services.DataManager
            .GetSources(DbKeys.Maps)
            .Where(s => s is FileSource)
            .Cast<FileSource>()
            .Where(s => s.Paths.Contains(item.FullPath))
            .Single();

        Services.DataManager.UnregisterSource(DbKeys.Maps.ToString(), source);

        Services.DataManager.UpdateData();

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
}

public partial class SettingsViewModel
{
    public SettingsViewModel(DesignDataDependencyResolver resolver)
    {
        SnapshotDirectory = "C:\\Users\\User\\AppData\\Roaming\\FootprintViewer\\Client\\Snapshots";

        _dataManager = resolver.GetService<IDataManager>();

        NextCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Observable
                .Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.1));

            Close(DialogResultKind.Normal);
        });

        _snapshotExtensions = AvailableExtensions();

        SelectedSnapshotExtension = _snapshotExtensions.First();

        var sources = _dataManager.GetSources(DbKeys.Maps);

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
    }
}