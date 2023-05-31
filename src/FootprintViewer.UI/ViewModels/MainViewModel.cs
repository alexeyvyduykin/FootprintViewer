using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Layers;
using FootprintViewer.Services;
using FootprintViewer.StateMachines;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.AddPlannedSchedule;
using FootprintViewer.UI.ViewModels.Dialogs;
using FootprintViewer.UI.ViewModels.InfoPanel;
using FootprintViewer.UI.ViewModels.Navigation;
using FootprintViewer.UI.ViewModels.Settings;
using FootprintViewer.UI.ViewModels.SidePanel;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using FootprintViewer.UI.ViewModels.SidePanel.Tabs;
using FootprintViewer.UI.ViewModels.Tips;
using FootprintViewer.UI.ViewModels.ToolBar;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Nito.Disposables.Internals;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase, IStateCommands
{
    private readonly IMapService _mapService;
    private readonly InfoPanelViewModel _infoPanel;
    private readonly InfoPanelViewModel _clickInfoPanel;
    private readonly ScaleMapBar _scaleMapBar;
    private readonly MapToolsViewModel _mapTools;
    private ISelector? _selector;
    private double _lastScreenPointX = 0;
    private double _lastScreenPointY = 0;

    public MainViewModel()
    {
        DialogScreen = new DialogScreenViewModel();

        FullScreen = new DialogScreenViewModel(NavigationTarget.FullScreen);

        MainScreen = new TargettedNavigationStack(NavigationTarget.HomeScreen);

        CompactDialogScreen = new DialogScreenViewModel(NavigationTarget.CompactDialogScreen);

        NavigationState.Register(MainScreen, DialogScreen, FullScreen, CompactDialogScreen);

        _mapService = Services.Locator.GetRequiredService<IMapService>();

        Map = _mapService.Map;

        Moved = ReactiveCommand.Create<(double, double)>(MovedImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _infoPanel = new InfoPanelViewModel();

        _clickInfoPanel = new InfoPanelViewModel();

        _scaleMapBar = new ScaleMapBar();

        _mapTools = new MapToolsViewModel();

        _mapService.State.Configure(this);

        IsMainContentEnabled = this.WhenAnyValue(
            s => s.DialogScreen.IsDialogOpen,
            s => s.FullScreen.IsDialogOpen,
            (dialogIsOpen, fullScreenIsOpen) => !(dialogIsOpen || fullScreenIsOpen))
            .ObserveOn(RxApp.MainThreadScheduler);

        this.WhenAnyValue(
            s => s.DialogScreen.CurrentPage,
            s => s.FullScreen.CurrentPage,
            s => s.MainScreen.CurrentPage,
            (dialog, fullScreenDialog, mainScreen) => dialog ?? fullScreenDialog ?? mainScreen)
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(page => page.SetActive())
            .Subscribe();

        Connection = ReactiveCommand.CreateFromTask(ConnectionImpl);

        Settings = ReactiveCommand.CreateFromTask(SettingsImpl);

        RegisterViewModels();
    }

    public static MainViewModel Instance { get; } = new();

    public void Initialize()
    {

    }

    public TargettedNavigationStack MainScreen { get; }

    private DialogScreenViewModel DialogScreen { get; set; }

    public DialogScreenViewModel FullScreen { get; set; }

    private DialogScreenViewModel CompactDialogScreen { get; set; }

    public ReactiveCommand<(double, double), Unit> Moved { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public ReactiveCommand<Unit, Unit> Connection { get; }

    public ReactiveCommand<Unit, Unit> Settings { get; }

    private void RegisterViewModels()
    {
        var satelliteTabViewModel = new SatelliteTabViewModel();
        var groundTargetTabViewModel = new GroundTargetTabViewModel();
        var footprintTabViewModel = new FootprintTabViewModel();
        var userGeometryTabViewModel = new UserGeometryTabViewModel();
        var groundStationTabViewModel = new GroundStationTabViewModel();
        var plannedScheduleTabViewModel = new PlannedScheduleTabViewModel();

        ToolBar = new ToolBarViewModel();

        SidePanel = new SidePanelViewModel()
        {
            Tabs = new List<SidePanelTabViewModel>(new SidePanelTabViewModel[]
            {
                satelliteTabViewModel,
                groundStationTabViewModel,
                groundTargetTabViewModel,
                footprintTabViewModel,
                userGeometryTabViewModel,
                plannedScheduleTabViewModel,
            }),
            ActionTabs = new()
            {
                new(nameof(AddPlannedSchedulePageViewModel), Connection),
                new(nameof(SettingsViewModel), Settings),
            }
        };
    }

    private async Task ConnectionImpl()
    {
        await DialogScreen.NavigateDialogAsync(new AddPlannedSchedulePageViewModel());
    }

    private async Task SettingsImpl()
    {
        await DialogScreen.NavigateDialogAsync(new SettingsViewModel());
    }

    private void MovedImpl((double, double) screenPosition)
    {
        var (x, y) = screenPosition;

        _lastScreenPointX = x;

        _lastScreenPointY = y;

        if (Tip != null)
        {
            Tip.X = x + 20;
            Tip.Y = y;

            if (Tip.IsVisible == false)
            {
                Tip.IsVisible = true;
            }
        }
    }

    private void LeaveImpl()
    {
        if (Tip != null)
        {
            Tip.IsVisible = false;
        }
    }

    private void ShowTip(object? content)
    {
        Tip ??= new Tip();

        Tip.X = _lastScreenPointX + 20;

        Tip.Y = _lastScreenPointY;

        Tip.IsVisible = true;

        Tip.Content = content;
    }

    private void HideTip()
    {
        Tip = null;
    }

    public async Task InitAsync()
    {
        var maps = await Services.Locator.GetRequiredService<ILocalStorageService>()
            .GetValuesAsync<MapResource>(DbKeys.Maps.ToString());

        var item = maps.FirstOrDefault();

        if (item != null)
        {
            Services.Locator.GetRequiredService<IMapService>().Map.SetWorldMapLayer(item);
        }

        Services.Locator.GetRequiredService<IMapService>().Map.Layers
            .ToList()
            .Where(s => s is DynamicLayer dynamicLayer && dynamicLayer.DataSource is IDynamic)
            .Select(s => (IDynamic)((DynamicLayer)s).DataSource!)
            .ToList()
            .ForEach(s => s.DataHasChanged());
    }

    private async Task OpenInfoPanel(ILayer layer, IFeature feature)
    {
        var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        InfoPanelItemViewModel? panel = layer?.Name switch
        {
            //nameof(LayerType.Footprint) =>
            //    (await Services.DataManager.GetDataAsync<PlannedScheduleResult>(nameof(DbKeys.PlannedSchedules))).FirstOrDefault()?.
            //        .Where(s => Equals(s.Name, feature?["Name"]))
            //        .Select(s => new FootprintViewModel(s))
            //        .Select(s => new FootprintClickInfoPanel(s))
            //        .FirstOrDefault(),
            nameof(LayerType.GroundTarget) =>
                (await localStorage.GetValuesAsync<PlannedScheduleResult>(nameof(DbKeys.PlannedSchedules))).FirstOrDefault()?.GroundTargets
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new GroundTargetViewModel(s))
                    .Select(s => GroundTargetInfoPanelItemViewModel.Create(s))
                    .FirstOrDefault() ?? null,
            nameof(LayerType.User) =>
                (await localStorage.GetValuesAsync<UserGeometry>(nameof(DbKeys.UserGeometries)))
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new UserGeometryViewModel(s))
                    .Select(s => UserGeometryInfoPanelItemViewModel.Create(s))
                    .FirstOrDefault(),
            _ => null
        };

        if (panel != null)
        {
            ClickInfoPanel.Show(panel);
        }
    }

    private void CloseInfoPanel(ILayer layer)
    {
        string? key = layer?.Name switch
        {
            nameof(LayerType.Footprint) => "Footprint",
            nameof(LayerType.GroundTarget) => "GroundTarget",
            nameof(LayerType.User) => "UserGeometry",
            _ => null
        };

        if (key != null)
        {
            ClickInfoPanel.CloseAll(key);
        }
    }

    private void EditFeature(IFeature? feature)
    {
        if (feature is GeometryFeature gf)
        {
            Observable.Start(async () =>
            {
                if (gf.Fields.Contains("Name") == true)
                {
                    var name = (string)gf["Name"]!;

                    var geometry = gf.Geometry!;

                    var model = new UserGeometry()
                    {
                        Geometry = geometry
                    };

                    var key = DbKeys.UserGeometries.ToString();

                    var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

                    await localStorage.TryEditAsync(key, name, model);
                }
            },
            RxApp.TaskpoolScheduler);
        }
    }

    private void AddUserGeometry(IFeature feature, UserGeometryType type)
    {
        if (feature is GeometryFeature gf)
        {
            var name = GenerateName(type);

            gf["Name"] = name;

            Observable.Start(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = type,
                    Name = name,
                    Geometry = gf.Geometry
                };

                var key = DbKeys.UserGeometries.ToString();

                var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

                await localStorage.TryAddAsync(key, model);
            },
            RxApp.TaskpoolScheduler);
        }

        static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }
    }

    private InfoPanelItemViewModel CreateAOIPanel(GeometryFeature feature)
    {
        var center = SphericalMercator.ToLonLat(feature.Geometry!.Centroid.ToMPoint());
        var area = feature.Geometry?.Area() ?? 0.0;// designer.Area();

        var descr = $"{FormatHelper.ToArea(area)} | {FormatHelper.ToCoordinate(center.X, center.Y)}";

        var panel = InfoPanelItemViewModel.Create("AOI", descr);

        panel.Close.Subscribe(_ =>
        {
            _mapService.AOI.Reset();

            HideTip();

            //ToolBar.Uncheck(); 

            _mapService.State.Reset();
        });

        return panel;
    }

    private InfoPanelItemViewModel CreateRoutePanel(IDesigner designer)
    {
        var distance = designer.Distance();

        var panel = InfoPanelItemViewModel.Create("Route", FormatHelper.ToDistance(distance));

        panel.Close.Subscribe(_ =>
        {
            var layer = Services.Locator.GetRequiredService<IMapService>().Map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.ClearRoute();

            HideTip();

            //ToolBar.Uncheck(); 

            _mapService.State.Reset();
        });

        return panel;
    }

    public Map Map { get; private set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = Mapsui.Interactivity.UI.States.Default;

    public SidePanelViewModel SidePanel { get; private set; }

    public InfoPanelViewModel InfoPanel => _infoPanel;

    public InfoPanelViewModel ClickInfoPanel => _clickInfoPanel;

    public ToolBarViewModel ToolBar { get; private set; }

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    public MapToolsViewModel MapTools => _mapTools;

    [Reactive]
    public ITip? Tip { get; set; }

    public IObservable<bool> IsMainContentEnabled { get; }
}