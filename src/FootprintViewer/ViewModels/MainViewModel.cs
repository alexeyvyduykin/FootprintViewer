using FootprintViewer.Data;
using FootprintViewer.Layers;
using InteractiveGeometry;
using InteractiveGeometry.UI;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IReadonlyDependencyResolver _dependencyResolver;
        private readonly Map _map;
        private readonly InfoPanel _infoPanel;
        private readonly InfoPanel _clickInfoPanel;
        private readonly SidePanel _sidePanel;
        private readonly CustomToolBar _customToolBar;
        private readonly FootprintObserver _footprintObserver;
        private readonly GroundTargetViewer _groundTargetViewer;
        private readonly UserGeometryViewer _userGeometryViewer;
        private readonly SceneSearch _sceneSearch;
        private readonly IUserLayerSource _userLayerSource;
        private readonly IFootprintLayerSource _footprintSource;
        private readonly IEditLayerSource _editSource;
        private readonly ScaleMapBar _scaleMapBar;

        private ISelectDecorator? _footprintSelectDecorator;
        private ISelectDecorator? _groundTargetSelectDecorator;
        private ISelectDecorator? _userGeometrySelectDecorator;
        private ISelectScaleDecorator? _selectScaleDecorator;
        private ISelectTranslateDecorator? _selectTranslateDecorator;
        private ISelectRotateDecorator? _selectRotateDecorator;
        private ISelectEditDecorator? _selectEditDecorator;

        public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            var factory = dependencyResolver.GetExistingService<ProjectFactory>();
            // TODO: make _map as IMap
            _map = (Map)dependencyResolver.GetExistingService<IMap>();
            MapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
            _sidePanel = dependencyResolver.GetExistingService<SidePanel>();
            _customToolBar = dependencyResolver.GetExistingService<CustomToolBar>();
            _userLayerSource = dependencyResolver.GetExistingService<IUserLayerSource>();
            _footprintSource = dependencyResolver.GetExistingService<IFootprintLayerSource>();
            _editSource = dependencyResolver.GetExistingService<IEditLayerSource>();
            _footprintObserver = dependencyResolver.GetExistingService<FootprintObserver>();
            _groundTargetViewer = dependencyResolver.GetExistingService<GroundTargetViewer>();
            _userGeometryViewer = dependencyResolver.GetExistingService<UserGeometryViewer>();
            _sceneSearch = dependencyResolver.GetExistingService<SceneSearch>();

            _infoPanel = factory.CreateInfoPanel();

            _clickInfoPanel = factory.CreateInfoPanel();

            _scaleMapBar = factory.CreateScaleMapBar();

            AOIChanged += (s, e) =>
            {
                if (s != null)
                {
                    if (s is Geometry geometry)
                    {
                        _sceneSearch.SetAOI(geometry);
                    }
                }
                else
                {
                    _sceneSearch.ResetAOI();
                }
            };

            ActualController = new DefaultController();

            _customToolBar.ZoomIn.Click.Subscribe(_ => MapNavigator.ZoomIn());
            _customToolBar.ZoomOut.Click.Subscribe(_ => MapNavigator.ZoomOut());

            _customToolBar.AddRectangle.Activate.Subscribe(_ => RectangleCommand());
            _customToolBar.AddRectangle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.AddPolygon.Activate.Subscribe(_ => PolygonCommand());
            _customToolBar.AddPolygon.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.AddCircle.Activate.Subscribe(_ => CircleCommand());
            _customToolBar.AddCircle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.RouteDistance.Activate.Subscribe(_ => RouteCommand());
            _customToolBar.RouteDistance.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.SelectGeometry.Activate.Subscribe(_ => SelectCommand());
            _customToolBar.SelectGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.TranslateGeometry.Activate.Subscribe(_ => TranslateCommand());
            _customToolBar.TranslateGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.RotateGeometry.Activate.Subscribe(_ => RotateCommand());
            _customToolBar.RotateGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.ScaleGeometry.Activate.Subscribe(_ => ScaleCommand());
            _customToolBar.ScaleGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.EditGeometry.Activate.Subscribe(_ => EditCommand());
            _customToolBar.EditGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Point.Activate.Subscribe(_ => DrawingPointCommand());
            _customToolBar.Point.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Rectangle.Activate.Subscribe(_ => DrawingRectangleCommand());
            _customToolBar.Rectangle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Circle.Activate.Subscribe(_ => DrawingCircleCommand());
            _customToolBar.Circle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Polygon.Activate.Subscribe(_ => DrawingPolygonCommand());
            _customToolBar.Polygon.Deactivate.Subscribe(_ => ResetInteractivity());

            _footprintObserver.ClickOnItem.Subscribe(s =>
            {
                var name = s?.Name;

                if (string.IsNullOrEmpty(name) == false)
                {
                    var feature = _footprintSource.GetFeature(name);

                    if (feature != null)
                    {
                        _footprintSelectDecorator ??= CreateFootprintSelector();

                        _footprintSelectDecorator.SelectFeature((BaseFeature)feature);
                    }

                    return;
                }
            });

            LoadingProviders = ReactiveCommand.CreateFromTask(LoadingProvidersAsync);

            var command = ReactiveCommand.CreateFromObservable<Unit, Unit>(_ => Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1)));

            command.Execute().InvokeCommand(LoadingProviders);
        }

        private ReactiveCommand<Unit, Unit> LoadingProviders { get; }

        private async Task LoadingProvidersAsync()
        {
            await Task.Run(() => _dependencyResolver.GetExistingService<IProvider<MapResource>>().Loading.Execute().Subscribe());
            await Task.Run(() => _dependencyResolver.GetExistingService<IEditableProvider<UserGeometryInfo>>().Loading.Execute().Subscribe());
            await Task.Run(() => _dependencyResolver.GetExistingService<IProvider<FootprintInfo>>().Loading.Execute().Subscribe());
            await Task.Run(() => _dependencyResolver.GetExistingService<IProvider<SatelliteInfo>>().Loading.Execute().Subscribe());
            await Task.Run(() => _dependencyResolver.GetExistingService<IProvider<GroundTargetInfo>>().Loading.Execute().Subscribe());
            await Task.Run(() => _dependencyResolver.GetExistingService<IProvider<GroundStationInfo>>().Loading.Execute().Subscribe());
        }

        private void ResetInteractivity()
        {
            if (_selectTranslateDecorator != null)
            {
                _selectTranslateDecorator.Dispose();
                _selectTranslateDecorator = null;
            }

            if (_selectScaleDecorator != null)
            {
                _selectScaleDecorator.Dispose();
                _selectScaleDecorator = null;
            }

            if (_selectRotateDecorator != null)
            {
                _selectRotateDecorator.Dispose();
                _selectRotateDecorator = null;
            }

            if (_selectEditDecorator != null)
            {
                _selectEditDecorator.Dispose();
                _selectEditDecorator = null;
            }

            if (_footprintSelectDecorator != null)
            {
                _footprintSelectDecorator.Dispose();
                _footprintSelectDecorator = null;
            }

            if (_groundTargetSelectDecorator != null)
            {
                _groundTargetSelectDecorator.Dispose();
                _groundTargetSelectDecorator = null;
            }

            if (_userGeometrySelectDecorator != null)
            {
                _userGeometrySelectDecorator.Dispose();
                _userGeometrySelectDecorator = null;
            }

            Tip = null;

            ActualController = new DefaultController();
        }

        public event EventHandler? AOIChanged;

        private void RectangleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateRectangleDesigner(Map);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip!.Text = "Отпустите клавишу мыши для завершения рисования";

                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
            };

            designer.EndCreating += (s, e) =>
            {
                var feature = designer.Feature.Copy();

                _editSource.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIRectangle.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(designer));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void PolygonCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreatePolygonDesigner(Map);

            designer.BeginCreating += (s, e) =>
            {
                Tip!.Text = "Нажмите, чтобы продолжить рисование фигуры";
            };

            designer.Creating += (s, e) =>
            {
                var area = designer.Area();

                if (area != 0.0)
                {
                    Tip!.Text = "Щелкните по первой точке, чтобы закрыть эту фигуру";
                    Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                }
            };

            designer.EndCreating += (s, e) =>
            {
                var feature = designer.Feature.Copy();

                _editSource.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIPolygon.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(designer));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать полигон" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void CircleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateCircleDesigner(Map);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip!.Text = "Отпустите клавишу мыши для завершения рисования";
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
            };

            designer.EndCreating += (s, e) =>
            {
                var feature = designer.Feature.Copy();

                _editSource.AddAOI(new InteractiveCircle(feature), Styles.FeatureType.AOICircle.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(designer));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать круг" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void RouteCommand()
        {
            var designer = (IRouteDesigner)new InteractiveFactory().CreateRouteDesigner(Map);

            designer.BeginCreating += (s, e) =>
            {
                var distance = designer.Distance();

                Tip!.Text = "";
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";
            };

            designer.Creating += (s, e) =>
            {
                InfoPanel.Show(CreateRoutePanel(designer));
            };

            designer.HoverCreating += (s, e) =>
            {
                var distance = designer.Distance();

                Tip!.Text = "";
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";
            };

            designer.EndCreating += (s, e) =>
            {
                var feature = designer.Feature.Copy();

                _editSource.AddRoute(new InteractiveRoute(feature), Styles.FeatureType.Route.ToString());

                Tip = null;

                _customToolBar.Uncheck();
            };

            _editSource.ClearRoute();

            InfoPanel.CloseAll(typeof(RouteInfoPanel));

            Tip = new Tip() { Text = "Кликните, чтобы начать измерение" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private ISelectDecorator CreateFootprintSelector()
        {
            var footprintLayer = Map.GetLayer<ILayer>(LayerType.Footprint)!;

            var footprintSelectDecorator = new InteractiveFactory().CreateSelectDecorator(Map, footprintLayer);

            footprintSelectDecorator.Select += (s, e) =>
            {
                var decorator = (ISelectDecorator)s!;

                var feature = decorator.SelectedFeature!;

                if (feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;
                    var info = _footprintObserver.GetFootprintInfo(name);

                    if (info != null)
                    {
                        ClickInfoPanel.Show(new FootprintClickInfoPanel(info));
                    }

                    if (_footprintObserver.IsActive == true)
                    {
                        _footprintObserver.SelectFootprintInfo(name);
                    }
                }
            };

            footprintSelectDecorator.Unselect += (s, e) =>
            {
                ClickInfoPanel.CloseAll(typeof(FootprintClickInfoPanel));
            };

            return footprintSelectDecorator;
        }

        private ISelectDecorator? CreateGroundTargetSelector()
        {
            var groundTargetLayer = Map.GetLayer<ILayer>(LayerType.GroundTarget);

            if (groundTargetLayer == null)
            {
                return null;
            }

            var groundTargetSelectDecorator = new InteractiveFactory().CreateSelectDecorator(Map, groundTargetLayer);

            groundTargetSelectDecorator.Select += (s, e) =>
            {
                var decorator = (ISelectDecorator)s!;

                var feature = decorator.SelectedFeature!;

                if (feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;

                    Task.Run(async () =>
                    {
                        var infos = await _groundTargetViewer.GetGroundTargetInfoAsync(name);

                        var info = infos.FirstOrDefault();

                        if (info != null)
                        {
                            ClickInfoPanel.Show(new GroundTargetClickInfoPanel(info));
                        }
                    });
                }
            };

            groundTargetSelectDecorator.Unselect += (s, e) =>
            {
                ClickInfoPanel.CloseAll(typeof(GroundTargetClickInfoPanel));
            };

            return groundTargetSelectDecorator;
        }

        private ISelectDecorator? CreateUserGeometrySelector()
        {
            var userGeometryLayer = Map.GetLayer<ILayer>(LayerType.User);

            if (userGeometryLayer == null)
            {
                return null;
            }

            var userGeometrySelectDecorator = new InteractiveFactory().CreateSelectDecorator(Map, userGeometryLayer);

            userGeometrySelectDecorator.Select += (s, e) =>
            {
                var decorator = (ISelectDecorator)s!;

                var feature = decorator.SelectedFeature!;

                if (feature.Fields.Contains("Name"))
                {
                    var name = (string)feature["Name"]!;

                    Task.Run(async () =>
                    {
                        var infos = await _userGeometryViewer.GetUserGeometryInfoAsync(name);

                        var info = infos.FirstOrDefault();

                        if (info != null)
                        {
                            ClickInfoPanel.Show(new UserGeometryClickInfoPanel(info));
                        }
                    });
                }
            };

            userGeometrySelectDecorator.Unselect += (_s, e) =>
            {
                ClickInfoPanel.CloseAll(typeof(UserGeometryClickInfoPanel));
            };

            return userGeometrySelectDecorator;
        }

        private void SelectCommand()
        {
            ActualController = new DefaultController();

            _footprintSelectDecorator ??= CreateFootprintSelector();

            _groundTargetSelectDecorator ??= CreateGroundTargetSelector();

            _userGeometrySelectDecorator ??= CreateUserGeometrySelector();
        }

        private void ScaleCommand()
        {
            var userLayer = _map.GetLayer<ILayer>(LayerType.User);

            if (userLayer == null)
            {
                return;
            }

            IFeature? selectFeature = null;

            _selectScaleDecorator = new InteractiveFactory().CreateSelectScaleDecorator(Map, userLayer);

            _selectScaleDecorator.Select += (s, e) =>
            {
                selectFeature = (IFeature?)((ISelectScaleDecorator)s!).SelectedFeature;

                Behavior = new InteractiveBehavior(((ISelectScaleDecorator)s!).Scale!);
            };

            _selectScaleDecorator.Unselect += (s, e) =>
            {
                _userLayerSource.EditFeature(selectFeature!);
            };

            ActualController = new EditController();
        }

        private void TranslateCommand()
        {
            var userLayer = _map.GetLayer<ILayer>(LayerType.User);

            if (userLayer == null)
            {
                return;
            }

            IFeature? selectFeature = null;

            _selectTranslateDecorator = new InteractiveFactory().CreateSelectTranslateDecorator(Map, userLayer);

            _selectTranslateDecorator.Select += (s, e) =>
            {
                selectFeature = (IFeature?)((ISelectTranslateDecorator)s!).SelectedFeature;

                Behavior = new InteractiveBehavior(((ISelectTranslateDecorator)s!).Translate!);
            };

            _selectTranslateDecorator.Unselect += (s, e) =>
            {
                _userLayerSource.EditFeature(selectFeature!);
            };

            ActualController = new EditController();
        }

        private void RotateCommand()
        {
            var userLayer = _map.GetLayer<ILayer>(LayerType.User);

            if (userLayer == null)
            {
                return;
            }

            IFeature? selectFeature = null;

            _selectRotateDecorator = new InteractiveFactory().CreateSelectRotateDecorator(Map, userLayer);

            _selectRotateDecorator.Select += (s, e) =>
            {
                selectFeature = (IFeature?)((ISelectRotateDecorator)s!).SelectedFeature;

                Behavior = new InteractiveBehavior(((ISelectRotateDecorator)s!).Rotate!);
            };

            _selectRotateDecorator.Unselect += (s, e) =>
            {
                _userLayerSource.EditFeature(selectFeature!);
            };

            ActualController = new EditController();
        }

        private void EditCommand()
        {
            var userLayer = _map.GetLayer<ILayer>(LayerType.User);

            if (userLayer == null)
            {
                return;
            }

            IFeature? selectFeature = null;

            _selectEditDecorator = new InteractiveFactory().CreateSelectEditDecorator(Map, userLayer);

            _selectEditDecorator.Select += (s, e) =>
            {
                selectFeature = (IFeature?)((ISelectEditDecorator)s!).SelectedFeature;

                Behavior = new InteractiveBehavior(((ISelectEditDecorator)s!).Edit!);
            };

            _selectEditDecorator.Unselect += (s, e) =>
            {
                _userLayerSource.EditFeature(selectFeature!);
            };

            ActualController = new EditController();
        }

        private InfoPanelItem CreateAOIPanel(IAreaDesigner designer)
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
                _editSource.ResetAOI();

                AOIChanged?.Invoke(null, EventArgs.Empty);

                Tip = null;

                ToolBar.Uncheck();
            });

            return panel;
        }

        private InfoPanelItem CreateRoutePanel(IRouteDesigner designer)
        {
            var distance = designer.Distance();

            var panel = new RouteInfoPanel()
            {
                Text = FormatHelper.ToDistance(distance),
            };

            panel.Close.Subscribe(_ =>
            {
                _editSource.ClearRoute();

                Tip = null;

                ToolBar.Uncheck();
            });

            return panel;
        }

        private void DrawingPointCommand()
        {
            var designer = new InteractiveFactory().CreatePointDesigner(Map);

            designer.EndCreating += (s, e) =>
            {
                _userLayerSource.AddUserGeometry(designer.Feature.Copy(), Data.UserGeometryType.Point);

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите, чтобы нарисовать точку" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingRectangleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateRectangleDesigner(Map);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip!.Text = "Отпустите клавишу мыши для завершения рисования";

                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayerSource.AddUserGeometry(designer.Feature.Copy(), Data.UserGeometryType.Rectangle);

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingCircleCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreateCircleDesigner(Map);

            designer.HoverCreating += (s, e) =>
            {
                var area = designer.Area();

                Tip!.Text = "Отпустите клавишу мыши для завершения рисования";
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayerSource.AddUserGeometry(designer.Feature.Copy(), Data.UserGeometryType.Circle);

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать круг" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingRouteCommand()
        {
            var designer = (IRouteDesigner)new InteractiveFactory().CreateRouteDesigner(Map);

            designer.HoverCreating += (s, e) =>
            {
                var distance = designer.Distance();

                Tip!.Text = "";
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";
            };

            designer.EndCreating += (s, e) =>
            {
                // TODO: user geometry route not enable
                //_userLayerSource.AddFeature(designer.Feature.Copy());

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Кликните, чтобы начать измерение" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void DrawingPolygonCommand()
        {
            var designer = (IAreaDesigner)new InteractiveFactory().CreatePolygonDesigner(Map);

            designer.BeginCreating += (s, e) =>
            {
                Tip!.Text = "Нажмите, чтобы продолжить рисование фигуры";
            };

            designer.Creating += (s, e) =>
            {
                var area = designer.Area();

                if (area != 0.0)
                {
                    Tip!.Text = "Щелкните по первой точке, чтобы закрыть эту фигуру";
                    Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                }
            };

            designer.EndCreating += (s, e) =>
            {
                _userLayerSource.AddUserGeometry(designer.Feature.Copy(), Data.UserGeometryType.Polygon);

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать полигон" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        public Map Map => _map;

        public SidePanel SidePanel => _sidePanel;

        public InfoPanel InfoPanel => _infoPanel;

        public InfoPanel ClickInfoPanel => _clickInfoPanel;

        public CustomToolBar ToolBar => _customToolBar;

        public ScaleMapBar ScaleMapBar => _scaleMapBar;

        [Reactive]
        public IMapNavigator MapNavigator { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Tip? Tip { get; set; }

        [Reactive]
        public IInteractiveBehavior? Behavior { get; set; }
    }

    public class MapLayer
    {
        public string? Name { get; set; }

        // public string? CRS { get; set; }

        public string? Format { get; set; }
    }
}
