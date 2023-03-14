using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels.Dialogs;
using FootprintViewer.ViewModels.Navigation;
using FootprintViewer.ViewModels.Settings;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Items;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using FootprintViewer.ViewModels.Timelines;
using FootprintViewer.ViewModels.Tips;
using FootprintViewer.ViewModels.ToolBar;
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
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;
    private readonly Map _map;
    private readonly StateMachines.MapState _mapState;
    private readonly AreaOfInterest _areaOfInterest;
    private readonly InfoPanel _infoPanel;
    private readonly InfoPanel _clickInfoPanel;
    private readonly SidePanelViewModel _sidePanel;
    private readonly BottomPanel _bottomPanel;
    private readonly ToolBarViewModel _toolBar;
    private readonly FootprintTabViewModel _footprintTab;
    private readonly GroundTargetTabViewModel _groundTargetTab;
    private readonly UserGeometryTabViewModel _userGeometryTab;
    private readonly ScaleMapBar _scaleMapBar;
    private ISelector? _selector;
    private readonly IDataManager _dataManager;
    private readonly FeatureManager _featureManager;
    private double _lastScreenPointX = 0;
    private double _lastScreenPointY = 0;

    public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        DialogScreen = new DialogScreenViewModel();

        FullScreen = new DialogScreenViewModel(NavigationTarget.FullScreen);

        MainScreen = new TargettedNavigationStack(NavigationTarget.HomeScreen);

        NavigationState.Register(MainScreen, DialogScreen, FullScreen);

        _dependencyResolver = dependencyResolver;
        var factory = dependencyResolver.GetExistingService<ProjectFactory>();
        // TODO: make _map as IMap
        _map = (Map)dependencyResolver.GetExistingService<IMap>();
        _mapState = dependencyResolver.GetExistingService<StateMachines.MapState>();

        MapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        _areaOfInterest = dependencyResolver.GetExistingService<AreaOfInterest>();
        _sidePanel = dependencyResolver.GetExistingService<SidePanelViewModel>();
        _toolBar = dependencyResolver.GetExistingService<ToolBarViewModel>();
        _footprintTab = dependencyResolver.GetExistingService<FootprintTabViewModel>();
        _groundTargetTab = dependencyResolver.GetExistingService<GroundTargetTabViewModel>();
        _userGeometryTab = dependencyResolver.GetExistingService<UserGeometryTabViewModel>();
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        _featureManager = dependencyResolver.GetExistingService<FeatureManager>();

        Moved = ReactiveCommand.Create<(double, double)>(MovedImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _infoPanel = factory.CreateInfoPanel();

        _clickInfoPanel = factory.CreateInfoPanel();

        _scaleMapBar = factory.CreateScaleMapBar();

        _bottomPanel = factory.CreateBottomPanel();

        _toolBar.ZoomIn.SubscribeAsync(MapNavigator.ZoomIn);
        _toolBar.ZoomOut.SubscribeAsync(MapNavigator.ZoomOut);

        _mapState.ResetObservable.Subscribe(_ => Reset());
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

        Observable.StartAsync(InitAsync, RxApp.MainThreadScheduler);
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

    private async Task ConnectionImpl()
    {
        var dataManager = _dependencyResolver.GetExistingService<IDataManager>();

        var connectionDialog = new ConnectionViewModel(_dependencyResolver);

        _ = await DialogScreen.NavigateDialogAsync(connectionDialog);

        dataManager.UpdateData();
    }

    private async Task SettingsImpl()
    {
        var dataManager = _dependencyResolver.GetExistingService<IDataManager>();

        var settingsDialog = new SettingsViewModel(_dependencyResolver);

        _ = await DialogScreen.NavigateDialogAsync(settingsDialog);

        dataManager.UpdateData();
    }

    private async Task TimelinesImpl()
    {
        var timelinesDialog = new TimelinesViewModel(_dependencyResolver);

        _ = await FullScreen.NavigateDialogAsync(timelinesDialog);
    }

    private async Task TimelinesOldImpl()
    {
        var timelinesOldDialog = new TimelinesOldViewModel(_dependencyResolver);

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

    private async Task InitAsync()
    {
        var maps = await _dataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());
        var item = maps.FirstOrDefault();
        if (item != null)
        {
            _map.SetWorldMapLayer(item);
        }

        _map.Layers
            .ToList()
            .Where(s => s is DynamicLayer dynamicLayer && dynamicLayer.DataSource is IDynamic)
            .Select(s => (IDynamic)((DynamicLayer)s).DataSource!)
            .ToList()
            .ForEach(s => s.DataHasChanged());
    }

    private void Reset()
    {
        Interactive?.Cancel();
        Interactive = null;

        _selector = null;

        State = States.Default;

        HideTip();
    }

    private void RectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Rectangle, s)));

        designer.EndCreating
            .Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            HideTip();

            InfoPanel.Show(CreateAOIPanel(s));

            _areaOfInterest.Update(feature, FeatureType.AOIRectangle);

            _mapState.Reset();
        });

        ShowTip(CustomTipViewModel.Init(TipTarget.Rectangle));

        Interactive = designer;

        State = States.Drawing;
    }

    private void PolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Polygon)));

        designer.Creating
            .Select(s => s.Area())
            .Where(s => s != 0.0)
            .Select(s => FormatHelper.ToArea(s))
            .Subscribe(s => ShowTip(CustomTipViewModel.Creating(TipTarget.Polygon, s)));

        designer.EndCreating
            .Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            HideTip();

            InfoPanel.Show(CreateAOIPanel(s));

            _areaOfInterest.Update(feature, FeatureType.AOIPolygon);

            _mapState.Reset();
        });

        ShowTip(CustomTipViewModel.Init(TipTarget.Polygon));

        Interactive = designer;

        State = States.Drawing;
    }

    private void CircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Circle, s)));

        designer.EndCreating
            .Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            HideTip();

            InfoPanel.Show(CreateAOIPanel(s));

            _areaOfInterest.Update(feature, FeatureType.AOICircle);

            _mapState.Reset();
        });

        ShowTip(CustomTipViewModel.Init(TipTarget.Circle));

        Interactive = designer;

        State = States.Drawing;
    }

    private void RouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

        designer.BeginCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Route, s)));

        designer.Creating
            .Subscribe(s => InfoPanel.Show(CreateRoutePanel(s)));

        designer.HoverCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Route, s)));

        designer.EndCreating
            .Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            layer?.AddRoute(new InteractiveRoute(feature), FeatureType.Route.ToString());

            HideTip();

            _mapState.Reset();
        });

        layer?.ClearRoute();

        InfoPanel.CloseAll(typeof(RouteInfoPanel));

        ShowTip(CustomTipViewModel.Init(TipTarget.Route));

        Interactive = designer;

        State = States.Drawing;
    }

    private void SelectCommand()
    {
        var types = new[] { LayerType.Footprint, LayerType.GroundTarget, LayerType.User };

        var availableLayers = types
            .Select(s => Map.GetLayer(s))
            .WhereNotNull()
            .ToArray();

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .AvailableFor(availableLayers)
            .Build();

        _selector.Select
            .Subscribe(async s =>
            {
                SelectFeature(s);
                await OpenInfoPanel(s);
            });

        _selector.Unselect
            .Subscribe(s =>
            {
                UnselectFeature(s);
                CloseInfoPanel(s);
            });

        _selector.HoverBegin
            .Subscribe(s => EnterFeature(s));

        _selector.HoverEnd
            .Subscribe(s => LeaveFeature(s));

        Interactive = _selector;

        State = States.Selecting;
    }

    private void EnterFeature(ISelector selector)
    {
        _featureManager
            .OnLayer(selector.PointeroverLayer)
            .Enter(selector.HoveringFeature);
    }

    private void LeaveFeature(ISelector selector)
    {
        _featureManager
            .OnLayer(selector.PointeroverLayer)
            .Leave();
    }

    private void SelectFeature(ISelector selector)
    {
        _featureManager
            .OnLayer(selector.SelectedLayer)
            .Select(selector.SelectedFeature);
    }

    private void UnselectFeature(ISelector selector)
    {
        _featureManager
            .OnLayer(selector.SelectedLayer)
            .Unselect();
    }

    private async Task OpenInfoPanel(ISelector selector)
    {
        var feature = selector.SelectedFeature;
        var layer = selector.SelectedLayer;

        InfoPanelItem? panel = layer?.Name switch
        {
            nameof(LayerType.Footprint) =>
                (await _dataManager.GetDataAsync<Footprint>(nameof(DbKeys.Footprints)))
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new FootprintViewModel(s))
                    .Select(s => new FootprintClickInfoPanel(s))
                    .FirstOrDefault(),
            nameof(LayerType.GroundTarget) =>
                (await _dataManager.GetDataAsync<GroundTarget>(nameof(DbKeys.GroundTargets)))
                    .Where(s => Equals(s.Name, feature?["Name"]))
                    .Select(s => new GroundTargetViewModel(s))
                    .Select(s => new GroundTargetClickInfoPanel(s))
                    .FirstOrDefault(),
            nameof(LayerType.User) =>
                (await _dataManager.GetDataAsync<UserGeometry>(nameof(DbKeys.UserGeometries)))
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

    private void ScaleCommand()
    {
        var userLayer = _map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<ScaleDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        _selector.Unselect.Subscribe(s =>
        {
            Interactive = s;

            State = States.Selecting;

            EditFeature(s.SelectedFeature);
        });

        Interactive = _selector;

        State = States.Selecting;
    }

    private void TranslateCommand()
    {
        var userLayer = _map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<TranslateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        _selector.Unselect.Subscribe(s =>
        {
            Interactive = s;

            State = States.Selecting;

            EditFeature(s.SelectedFeature);
        });

        Interactive = _selector;

        State = States.Selecting;
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

                    await _dataManager.TryEditAsync(key, name, model);

                    _dataManager.ForceUpdateData(key);
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

                await _dataManager.TryAddAsync(key, model);

                _dataManager.ForceUpdateData(key);
            },
            RxApp.TaskpoolScheduler);
        }

        static string GenerateName(UserGeometryType type)
        {
            return $"{type}_{new string($"{Guid.NewGuid()}".Replace("-", "").Take(10).ToArray())}";
        }
    }

    private void RotateCommand()
    {
        var userLayer = _map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<RotateDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        _selector.Unselect.Subscribe(s =>
        {
            Interactive = s;
            State = States.Selecting;
            EditFeature(s.SelectedFeature);
        });

        Interactive = _selector;
        State = States.Selecting;
    }

    private void EditCommand()
    {
        var userLayer = _map.GetLayer<ILayer>(LayerType.User);

        if (userLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectDecorator<EditDecorator>()
            .AttachTo(Map)
            .WithSelector<Selector>()
            .AvailableFor(userLayer)
            .Build();

        ((IDecoratorSelector)_selector).DecoratorSelecting.Subscribe(s =>
        {
            Interactive = s;
            State = States.Editing;
        });

        _selector.Unselect.Subscribe(s =>
        {
            Interactive = s;
            State = States.Selecting;
            EditFeature(s.SelectedFeature);
        });

        Interactive = _selector;
        State = States.Selecting;
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
            var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.ClearRoute();

            HideTip();

            //ToolBar.Uncheck(); 

            _mapState.Reset();
        });

        return panel;
    }

    private void DrawingPointCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PointDesigner>()
            .AttachTo(Map)
            .Build();

        designer.EndCreating.Subscribe(s =>
        {
            AddUserGeometry(s.Feature.Copy(), UserGeometryType.Point);

            HideTip();

            _mapState.Reset();
        });

        ShowTip(CustomTipViewModel.Init(TipTarget.Point));

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingRectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Rectangle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s.Feature.Copy(), UserGeometryType.Rectangle);

                HideTip();

                _mapState.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Rectangle));

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingCircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToArea(s.Area()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Circle, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s.Feature.Copy(), UserGeometryType.Circle);

                HideTip();

                _mapState.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Circle));

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingRouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating
            .Select(s => FormatHelper.ToDistance(s.Distance()))
            .Subscribe(s => ShowTip(CustomTipViewModel.HoverCreating(TipTarget.Route, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                HideTip();

                _mapState.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Route));

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingPolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating
            .Subscribe(s => ShowTip(CustomTipViewModel.BeginCreating(TipTarget.Polygon)));

        designer.Creating
            .Select(s => s.Area())
            .Where(s => s != 0.0)
            .Select(s => FormatHelper.ToArea(s))
            .Subscribe(s => ShowTip(CustomTipViewModel.Creating(TipTarget.Polygon, s)));

        designer.EndCreating
            .Subscribe(s =>
            {
                AddUserGeometry(s.Feature.Copy(), UserGeometryType.Polygon);

                HideTip();

                _mapState.Reset();
            });

        ShowTip(CustomTipViewModel.Init(TipTarget.Polygon));

        Interactive = designer;

        State = States.Drawing;
    }

    public Map Map => _map;

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;

    public SidePanelViewModel SidePanel => _sidePanel;

    public InfoPanel InfoPanel => _infoPanel;

    public BottomPanel BottomPanel => _bottomPanel;

    public InfoPanel ClickInfoPanel => _clickInfoPanel;

    public ToolBarViewModel ToolBar => _toolBar;

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    [Reactive]
    public IMapNavigator MapNavigator { get; set; }

    [Reactive]
    public ITip? Tip { get; set; }

    public IObservable<bool> IsMainContentEnabled { get; }
}
