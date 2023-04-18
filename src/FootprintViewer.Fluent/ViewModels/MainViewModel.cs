using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using FootprintViewer.Fluent.ViewModels.InfoPanel;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Fluent.ViewModels.Settings;
using FootprintViewer.Fluent.ViewModels.SidePanel;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;
using FootprintViewer.Fluent.ViewModels.Timelines;
using FootprintViewer.Fluent.ViewModels.Tips;
using FootprintViewer.Fluent.ViewModels.ToolBar;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Interactivity;
using Mapsui.Interactivity.Extensions;
using Mapsui.Interactivity.UI;
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

namespace FootprintViewer.Fluent.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    private readonly StateMachines.MapState _mapState;
    private readonly AreaOfInterest _areaOfInterest;
    private readonly InfoPanelViewModel _infoPanel;
    private readonly InfoPanelViewModel _clickInfoPanel;
    private readonly BottomPanel _bottomPanel;
    private readonly ScaleMapBar _scaleMapBar;
    private ISelector? _selector;
    private double _lastScreenPointX = 0;
    private double _lastScreenPointY = 0;

    public MainViewModel()
    {
        DialogScreen = new DialogScreenViewModel();

        FullScreen = new DialogScreenViewModel(NavigationTarget.FullScreen);

        MainScreen = new TargettedNavigationStack(NavigationTarget.HomeScreen);

        NavigationState.Register(MainScreen, DialogScreen, FullScreen);

        _mapState = Services.MapState;

        MapNavigator = Services.MapNavigator;

        _areaOfInterest = Services.AreaOfInterest;

        Moved = ReactiveCommand.Create<(double, double)>(MovedImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _infoPanel = new InfoPanelViewModel();

        _clickInfoPanel = new InfoPanelViewModel();

        _scaleMapBar = new ScaleMapBar();

        _bottomPanel = new BottomPanel();

        _mapState.ResetObservable.Subscribe(_ => ResetCommand());
        _mapState.RectAOIObservable.Subscribe(_ => RectangleCommand());
        _mapState.PolygonAOIObservable.Subscribe(_ => PolygonCommand());
        _mapState.CircleAOIObservable.Subscribe(_ => CircleCommand());
        _mapState.RouteObservable.Subscribe(_ => RouteCommand());
        _mapState.SelectObservable.Subscribe(_ => SelectCommand());
        _mapState.TranslateObservable.Subscribe(_ => TranslateCommand());
        _mapState.RotateObservable.Subscribe(_ => RotateCommand());
        _mapState.ScaleObservable.Subscribe(_ => ScaleCommand());
        _mapState.EditObservable.Subscribe(_ => EditCommand());
        _mapState.PointObservable.Subscribe(_ => DrawingPointCommand());
        _mapState.RectObservable.Subscribe(_ => DrawingRectangleCommand());
        _mapState.CircleObservable.Subscribe(_ => DrawingCircleCommand());
        _mapState.PolygonObservable.Subscribe(_ => DrawingPolygonCommand());

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

        Timelines = ReactiveCommand.CreateFromTask(TimelinesImpl);

        TimelinesOld = ReactiveCommand.CreateFromTask(TimelinesOldImpl);

        RegisterViewModels();
    }

    public static MainViewModel Instance { get; } = new();

    public void Initialize()
    {

    }

    public TargettedNavigationStack MainScreen { get; }

    private DialogScreenViewModel DialogScreen { get; set; }

    private DialogScreenViewModel FullScreen { get; set; }

    public ReactiveCommand<(double, double), Unit> Moved { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public ReactiveCommand<Unit, Unit> Connection { get; }

    public ReactiveCommand<Unit, Unit> Settings { get; }

    public ReactiveCommand<Unit, Unit> Timelines { get; }

    public ReactiveCommand<Unit, Unit> TimelinesOld { get; }

    private void RegisterViewModels()
    {
        var satelliteTabViewModel = new SatelliteTabViewModel();
        var groundTargetTabViewModel = new GroundTargetTabViewModel();
        var footprintTabViewModel = new FootprintTabViewModel();
        var userGeometryTabViewModel = new UserGeometryTabViewModel();
        var groundStationTabViewModel = new GroundStationTabViewModel();
        var plannedScheduleTabViewModel = new PlannedScheduleTabViewModel();

        ToolBar = new ToolBarViewModel();

        ToolBar.ZoomIn.SubscribeAsync(MapNavigator.ZoomIn);
        ToolBar.ZoomOut.SubscribeAsync(MapNavigator.ZoomOut);

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
            })
        };
    }

    private async Task ConnectionImpl()
    {
        var connectionDialog = new ConnectionViewModel();

        _ = await DialogScreen.NavigateDialogAsync(connectionDialog);

        Services.DataManager.UpdateData();
    }

    private async Task SettingsImpl()
    {
        var settingsDialog = new SettingsViewModel();

        _ = await DialogScreen.NavigateDialogAsync(settingsDialog);

        Services.DataManager.UpdateData();
    }

    private async Task TimelinesImpl()
    {
        var timelinesDialog = new TimelinesViewModel();

        _ = await FullScreen.NavigateDialogAsync(timelinesDialog);
    }

    private async Task TimelinesOldImpl()
    {
        var timelinesOldDialog = new TimelinesOldViewModel();

        _ = await FullScreen.NavigateDialogAsync(timelinesOldDialog);
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
        var maps = await Services.DataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());
        var item = maps.FirstOrDefault();
        if (item != null)
        {
            Services.Map.SetWorldMapLayer(item);
        }

        Services.Map.Layers
            .ToList()
            .Where(s => s is DynamicLayer dynamicLayer && dynamicLayer.DataSource is IDynamic)
            .Select(s => (IDynamic)((DynamicLayer)s).DataSource!)
            .ToList()
            .ForEach(s => s.DataHasChanged());
    }

    private void EnterFeature(ISelector selector)
    {
        Services.FeatureManager
            .OnLayer(selector.PointeroverLayer)
            .Enter(selector.HoveringFeature);
    }

    private void LeaveFeature(ISelector selector)
    {
        Services.FeatureManager
            .OnLayer(selector.PointeroverLayer)
            .Leave();
    }

    private void SelectFeature(ISelector selector)
    {
        Services.FeatureManager
            .OnLayer(selector.SelectedLayer)
            .Select(selector.SelectedFeature);
    }

    private void UnselectFeature(ISelector selector)
    {
        Services.FeatureManager
            .OnLayer(selector.SelectedLayer)
            .Unselect();
    }

    private async Task OpenInfoPanel(ISelector selector)
    {
        var feature = selector.SelectedFeature;
        var layer = selector.SelectedLayer;

        InfoPanelItem? panel = layer?.Name switch
        {
            //nameof(LayerType.Footprint) =>
            //    (await _dataManager.GetDataAsync<Footprint>(nameof(DbKeys.Footprints)))
            //        .Where(s => Equals(s.Name, feature?["Name"]))
            //        .Select(s => new FootprintViewModel(s))
            //        .Select(s => new FootprintClickInfoPanel(s))
            //        .FirstOrDefault(),
            nameof(LayerType.GroundTarget) =>
                (await Services.DataManager.GetDataAsync<PlannedScheduleResult>(nameof(DbKeys.PlannedSchedules))).FirstOrDefault()?.GroundTargets
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new GroundTargetViewModel(s))
                    .Select(s => new GroundTargetClickInfoPanel(s))
                    .FirstOrDefault() ?? null,
            nameof(LayerType.User) =>
                (await Services.DataManager.GetDataAsync<UserGeometry>(nameof(DbKeys.UserGeometries)))
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new UserGeometryViewModel(s))
                    .Select(s => new UserGeometryClickInfoPanel(s))
                    .FirstOrDefault(),
            _ => null
        };

        if (panel != null)
        {
            ClickInfoPanel.Show(panel);
        }
    }

    private void CloseInfoPanel(ISelector selector)
    {
        Type? type = selector.SelectedLayer?.Name switch
        {
            nameof(LayerType.Footprint) => typeof(FootprintClickInfoPanel),
            nameof(LayerType.GroundTarget) => typeof(GroundTargetClickInfoPanel),
            nameof(LayerType.User) => typeof(UserGeometryClickInfoPanel),
            _ => null
        };

        if (type != null)
        {
            ClickInfoPanel.CloseAll(type);
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

                    await Services.DataManager.TryEditAsync(key, name, model);

                    Services.DataManager.ForceUpdateData(key);
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

                await Services.DataManager.TryAddAsync(key, model);

                Services.DataManager.ForceUpdateData(key);
            },
            RxApp.TaskpoolScheduler);
        }

        static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }
    }

    private InfoPanelItem CreateAOIPanel(IDesigner designer)
    {
        var center = SphericalMercator.ToLonLat(designer.Feature.Geometry!.Centroid.ToMPoint());
        var area = designer.Area();

        var descr = $"{FormatHelper.ToArea(area)} | {FormatHelper.ToCoordinate(center.X, center.Y)}";

        var panel = new AOIInfoPanel()
        {
            Text = descr,
        };

        panel.Close.Subscribe(_ =>
        {
            _areaOfInterest.Update(null);

            HideTip();

            //ToolBar.Uncheck(); 

            _mapState.Reset();
        });

        return panel;
    }

    private InfoPanelItem CreateRoutePanel(IDesigner designer)
    {
        var distance = designer.Distance();

        var panel = new RouteInfoPanel()
        {
            Text = FormatHelper.ToDistance(distance),
        };

        panel.Close.Subscribe(_ =>
        {
            var layer = Services.Map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.ClearRoute();

            HideTip();

            //ToolBar.Uncheck(); 

            _mapState.Reset();
        });

        return panel;
    }

    public Map Map => Services.Map;

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    public SidePanelViewModel SidePanel { get; private set; }// => _sidePanel;

    public InfoPanelViewModel InfoPanel => _infoPanel;

    public BottomPanel BottomPanel => _bottomPanel;

    public InfoPanelViewModel ClickInfoPanel => _clickInfoPanel;

    public ToolBarViewModel ToolBar { get; private set; }//=> _toolBar;

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    [Reactive]
    public IMapNavigator MapNavigator { get; set; }

    [Reactive]
    public ITip? Tip { get; set; }

    public IObservable<bool> IsMainContentEnabled { get; }
}
