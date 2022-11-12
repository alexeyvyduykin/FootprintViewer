﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.ViewModels.Dialogs;
using FootprintViewer.ViewModels.Navigation;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Items;
using FootprintViewer.ViewModels.SidePanel.Tabs;
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
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace FootprintViewer.ViewModels;

public class MainViewModel : RoutableViewModel
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;
    private readonly Map _map;
    private readonly InfoPanel _infoPanel;
    private readonly InfoPanel _clickInfoPanel;
    private readonly SidePanelViewModel _sidePanel;
    private readonly BottomPanel _bottomPanel;
    private readonly CustomToolBarViewModel _customToolBar;
    private readonly FootprintTabViewModel _footprintTab;
    private readonly GroundTargetTabViewModel _groundTargetTab;
    private readonly UserGeometryTabViewModel _userGeometryTab;
    private readonly FootprintPreviewTabViewModel _footprintPreviewTab;
    private readonly ScaleMapBar _scaleMapBar;
    private ISelector? _selector;
    private readonly IDataManager _dataManager;

    public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
        var factory = dependencyResolver.GetExistingService<ProjectFactory>();
        // TODO: make _map as IMap
        _map = (Map)dependencyResolver.GetExistingService<IMap>();
        MapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        _sidePanel = dependencyResolver.GetExistingService<SidePanelViewModel>();
        _customToolBar = dependencyResolver.GetExistingService<CustomToolBarViewModel>();
        _footprintTab = dependencyResolver.GetExistingService<FootprintTabViewModel>();
        _groundTargetTab = dependencyResolver.GetExistingService<GroundTargetTabViewModel>();
        _userGeometryTab = dependencyResolver.GetExistingService<UserGeometryTabViewModel>();
        _footprintPreviewTab = dependencyResolver.GetExistingService<FootprintPreviewTabViewModel>();
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        _infoPanel = factory.CreateInfoPanel();

        _clickInfoPanel = factory.CreateInfoPanel();

        _scaleMapBar = factory.CreateScaleMapBar();

        _bottomPanel = factory.CreateBottomPanel();

        AOIChanged += (s, e) =>
        {
            if (s != null)
            {
                if (s is Geometry geometry)
                {
                    _footprintPreviewTab.SetAOI(geometry);
                }
            }
            else
            {
                _footprintPreviewTab.ResetAOI();
            }
        };

        _customToolBar.ZoomIn.SubscribeAsync(() => Task.Run(() => MapNavigator.ZoomIn()));
        _customToolBar.ZoomOut.SubscribeAsync(() => Task.Run(() => MapNavigator.ZoomOut()));
        _customToolBar.AddRectangle.Subscribe(RectangleCommand, Reset);
        _customToolBar.AddPolygon.Subscribe(PolygonCommand, Reset);
        _customToolBar.AddCircle.Subscribe(CircleCommand, Reset);
        _customToolBar.RouteDistance.Subscribe(RouteCommand, Reset);
        _customToolBar.SelectGeometry.Subscribe(SelectCommand, Reset);
        _customToolBar.TranslateGeometry.Subscribe(TranslateCommand, Reset);
        _customToolBar.RotateGeometry.Subscribe(RotateCommand, Reset);
        _customToolBar.ScaleGeometry.Subscribe(ScaleCommand, Reset);
        _customToolBar.EditGeometry.Subscribe(EditCommand, Reset);
        _customToolBar.Point.Subscribe(DrawingPointCommand, Reset);
        _customToolBar.Rectangle.Subscribe(DrawingRectangleCommand, Reset);
        _customToolBar.Circle.Subscribe(DrawingCircleCommand, Reset);
        _customToolBar.Polygon.Subscribe(DrawingPolygonCommand, Reset);

        _footprintTab.TargetToMap.Subscribe(SelectFeatureImpl);

        IsMainContentEnabled = this.WhenAnyValue(s => s.DialogNavigationStack.IsDialogOpen, (s) => !s).ObserveOn(RxApp.MainThreadScheduler);

        this.WhenAnyValue(s => s.DialogNavigationStack.CurrentPage)
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(s => s.SetActive())
            .Subscribe();

        Observable.StartAsync(InitAsync, RxApp.MainThreadScheduler).Subscribe();
    }

    private async Task InitAsync()
    {
        var maps = await _dataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());
        var item = maps.FirstOrDefault();
        if (item != null)
        {
            _map.SetWorldMapLayer(item);
        }
    }

    public DialogNavigationStack DialogNavigationStack => DialogStack();

    private void SelectFeatureImpl(FootprintViewModel vm)
    {
        var layer = _map.GetLayer<Layer>(LayerType.Footprint);
        var provider = _dependencyResolver.GetExistingService<FootprintProvider>();
        var feature = provider?.Find(vm.Name, "Name");

        if (feature != null && layer != null)
        {
            _selector ??= new InteractiveBuilder().SelectSelector<Selector>().Build();

            _selector.Selected(feature, layer);
        }
    }

    private void Reset()
    {
        Interactive?.Cancel();
        Interactive = null;

        _selector = null;

        State = States.Default;

        Tip = null;
    }

    public event EventHandler? AOIChanged;

    private void RectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToArea(s.Area())));

        designer.EndCreating.Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIRectangle.ToString());

            Tip = null;

            InfoPanel.Show(CreateAOIPanel(s));

            AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreateRectangleTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void PolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating.Subscribe(s => Tip?.BeginCreating());

        designer.Creating.Subscribe(s =>
        {
            var area = s.Area();

            if (area != 0.0)
            {
                Tip?.Creating(FormatHelper.ToArea(area));
            }
        });

        designer.EndCreating.Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIPolygon.ToString());

            Tip = null;

            InfoPanel.Show(CreateAOIPanel(s));

            AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreatePolygonTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void CircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToArea(s.Area())));

        designer.EndCreating.Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.AddAOI(new InteractiveCircle(feature), Styles.FeatureType.AOICircle.ToString());

            Tip = null;

            InfoPanel.Show(CreateAOIPanel(s));

            AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreateCircleTip();

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

        designer.BeginCreating.Subscribe(s => Tip?.BeginCreating(FormatHelper.ToDistance(s.Distance())));

        designer.Creating.Subscribe(s => InfoPanel.Show(CreateRoutePanel(s)));

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToDistance(s.Distance())));

        designer.EndCreating.Subscribe(s =>
        {
            var feature = s.Feature.Copy();

            layer?.AddRoute(new InteractiveRoute(feature), Styles.FeatureType.Route.ToString());

            Tip = null;

            _customToolBar.Uncheck();
        });

        layer?.ClearRoute();

        InfoPanel.CloseAll(typeof(RouteInfoPanel));

        Tip = DrawingTips.CreateRouteTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void SelectCommand()
    {
        var footprintLayer = Map.GetLayer<ILayer>(LayerType.Footprint);
        var groundTargetLayer = Map.GetLayer<ILayer>(LayerType.GroundTarget);
        var userGeometryLayer = Map.GetLayer<ILayer>(LayerType.User);

        if (footprintLayer == null || groundTargetLayer == null || userGeometryLayer == null)
        {
            return;
        }

        _selector?.Cancel();

        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .AvailableFor(new[] { footprintLayer, groundTargetLayer, userGeometryLayer })
            .Build();

        _selector.Select.Subscribe(async s =>
        {
            var feature = s.SelectedFeature;
            var layer = s.SelectedLayer;

            if (string.Equals(layer?.Name, footprintLayer.Name) == true)
            {
                if (feature != null && feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;
                    var viewModels = await _footprintTab.GetFootprintViewModelsAsync(name);

                    var vm = viewModels.FirstOrDefault();

                    if (vm != null)
                    {
                        ClickInfoPanel.Show(new FootprintClickInfoPanel(vm));
                    }
                }
            }
            else if (string.Equals(layer?.Name, groundTargetLayer.Name) == true)
            {
                if (feature != null && feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;

                    await Task.Run(async () =>
                    {
                        var viewModels = await _groundTargetTab.GetGroundTargetViewModelsAsync(name);

                        var vm = viewModels.FirstOrDefault();

                        if (vm != null)
                        {
                            ClickInfoPanel.Show(new GroundTargetClickInfoPanel(vm));
                        }
                    });
                }
            }
            else if (string.Equals(layer?.Name, userGeometryLayer.Name) == true)
            {
                if (feature != null && feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;

                    await Task.Run(async () =>
                    {
                        var viewModels = await _userGeometryTab.GetUserGeometryViewModelsAsync(name);

                        var vm = viewModels.FirstOrDefault();

                        if (vm != null)
                        {
                            ClickInfoPanel.Show(new UserGeometryClickInfoPanel(vm));
                        }
                    });
                }
            }
        });

        _selector.Unselect.Subscribe(s =>
        {
            if (string.Equals(s.SelectedLayer?.Name, footprintLayer.Name) == true)
            {
                ClickInfoPanel.CloseAll(typeof(FootprintClickInfoPanel));
            }
            else if (string.Equals(s.SelectedLayer?.Name, groundTargetLayer.Name) == true)
            {
                ClickInfoPanel.CloseAll(typeof(GroundTargetClickInfoPanel));
            }
            else if (string.Equals(s.SelectedLayer?.Name, userGeometryLayer.Name) == true)
            {
                ClickInfoPanel.CloseAll(typeof(UserGeometryClickInfoPanel));
            }
        });

        Interactive = _selector;

        State = States.Selecting;
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
        //  var editableProvider = _dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

        if (feature is GeometryFeature gf)
        {
            Task.Run(async () =>
            {
                if (gf.Fields.Contains("Name") == true)
                {
                    var name = (string)gf["Name"]!;

                    var geometry = gf.Geometry!;

                    var model = new UserGeometry()
                    {
                        Geometry = geometry
                    };

                    //await editableProvider.EditAsync(name, model);

                    await _dataManager.TryEditAsync(DbKeys.UserGeometries.ToString(), name, model);
                }
            });
        }
    }

    private void AddUserGeometry(IFeature feature, UserGeometryType type)
    {
        //  var editableProvider = _dependencyResolver.GetExistingService<IEditableProvider<UserGeometry>>();

        if (feature is GeometryFeature gf)
        {
            var name = GenerateName(type);

            gf["Name"] = name;

            Task.Run(async () =>
            {
                var model = new UserGeometry()
                {
                    Type = type,
                    Name = name,
                    Geometry = gf.Geometry
                };

                //await editableProvider.AddAsync(model);

                await _dataManager.TryAddAsync(DbKeys.UserGeometries.ToString(), model);
            });
        }

        string GenerateName(UserGeometryType type)
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
            var layer = _map.GetLayer<EditLayer>(LayerType.Edit);

            layer?.ResetAOI();

            AOIChanged?.Invoke(null, EventArgs.Empty);

            Tip = null;

            ToolBar.Uncheck();
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

            Tip = null;

            ToolBar.Uncheck();
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
            AddUserGeometry(s.Feature.Copy(), Data.UserGeometryType.Point);

            Tip = null;

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreatePointTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingRectangleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RectangleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToArea(s.Area())));

        designer.EndCreating.Subscribe(s =>
        {
            AddUserGeometry(s.Feature.Copy(), Data.UserGeometryType.Rectangle);

            Tip = null;

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreateRectangleTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingCircleCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<CircleDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToArea(s.Area())));

        designer.EndCreating.Subscribe(s =>
        {
            AddUserGeometry(s.Feature.Copy(), Data.UserGeometryType.Circle);

            Tip = null;

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreateCircleTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingRouteCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<RouteDesigner>()
            .AttachTo(Map)
            .Build();

        designer.HoverCreating.Subscribe(s => Tip?.HoverCreating(FormatHelper.ToDistance(s.Distance())));

        designer.EndCreating.Subscribe(s =>
        {
            Tip = null;

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreateRouteTip();

        Interactive = designer;

        State = States.Drawing;
    }

    private void DrawingPolygonCommand()
    {
        var designer = new InteractiveBuilder()
            .SelectDesigner<PolygonDesigner>()
            .AttachTo(Map)
            .Build();

        designer.BeginCreating.Subscribe(s => Tip?.BeginCreating());

        designer.Creating.Subscribe(s =>
        {
            var area = s.Area();

            if (area != 0.0)
            {
                Tip?.Creating(FormatHelper.ToArea(area));
            }
        });

        designer.EndCreating.Subscribe(s =>
        {
            AddUserGeometry(s.Feature.Copy(), Data.UserGeometryType.Polygon);

            Tip = null;

            _customToolBar.Uncheck();
        });

        Tip = DrawingTips.CreatePolygonTip();

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

    public CustomToolBarViewModel ToolBar => _customToolBar;

    public ScaleMapBar ScaleMapBar => _scaleMapBar;

    [Reactive]
    public IMapNavigator MapNavigator { get; set; }

    [Reactive]
    public DrawingTip? Tip { get; set; }

    public IObservable<bool> IsMainContentEnabled { get; }
}
