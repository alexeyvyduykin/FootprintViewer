using FootprintViewer.Layers;
using InteractiveGeometry;
using InteractiveGeometry.UI;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly Map _map;
        private readonly InfoPanel _infoPanel;
        private readonly SidePanel _sidePanel;
        private readonly ProjectFactory _factory;
        private readonly CustomToolBar _customToolBar;
        private readonly FootprintObserver _footprintObserver;
        private readonly SceneSearch _sceneSearch;
        private readonly IUserLayerSource _userLayerSource;
        //     private GeometryFeature? _currentFeature;
        //private readonly IReadonlyDependencyResolver _dependencyResolver;
        //     private bool _isDirtyDecorator = false;

        public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            //_dependencyResolver = dependencyResolver;
            _factory = dependencyResolver.GetExistingService<ProjectFactory>();
            _map = dependencyResolver.GetExistingService<Map>();
            _sidePanel = dependencyResolver.GetExistingService<SidePanel>();
            _customToolBar = dependencyResolver.GetExistingService<CustomToolBar>();
            _userLayerSource = dependencyResolver.GetExistingService<IUserLayerSource>();
            _footprintObserver = dependencyResolver.GetExistingService<FootprintObserver>();
            _sceneSearch = dependencyResolver.GetExistingService<SceneSearch>();

            _infoPanel = _factory.CreateInfoPanel();

            _map.DataChanged += Map_DataChanged;

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

            _customToolBar.ZoomIn.Click.Subscribe(_ => ZoomInCommand());
            _customToolBar.ZoomOut.Click.Subscribe(_ => ZoomOutCommand());

            _customToolBar.AddRectangle.Activate.Subscribe(_ => RectangleCommand());
            _customToolBar.AddRectangle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.AddPolygon.Activate.Subscribe(_ => PolygonCommand());
            _customToolBar.AddPolygon.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.AddCircle.Activate.Subscribe(_ => CircleCommand());
            _customToolBar.AddCircle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.RouteDistance.Activate.Subscribe(_ => RouteCommand());
            _customToolBar.RouteDistance.Deactivate.Subscribe(_ => ResetInteractivity());

            //_customToolBar.SelectGeometry.Activate.Subscribe(_ => ActualController = new EditController());
            //_customToolBar.SelectGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            //_customToolBar.TranslateGeometry.Activate.Subscribe(_ => ActualController = new EditController());
            //_customToolBar.TranslateGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            //_customToolBar.RotateGeometry.Activate.Subscribe(_ => ActualController = new EditController());
            //_customToolBar.RotateGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            //_customToolBar.ScaleGeometry.Activate.Subscribe(_ => ActualController = new EditController());
            //_customToolBar.ScaleGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            //_customToolBar.EditGeometry.Activate.Subscribe(_ => ActualController = new EditController());
            //_customToolBar.EditGeometry.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Point.Activate.Subscribe(_ => DrawingPointCommand());
            _customToolBar.Point.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Rectangle.Activate.Subscribe(_ => DrawingRectangleCommand());
            _customToolBar.Rectangle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Circle.Activate.Subscribe(_ => DrawingCircleCommand());
            _customToolBar.Circle.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.Polygon.Activate.Subscribe(_ => DrawingPolygonCommand());
            _customToolBar.Polygon.Deactivate.Subscribe(_ => ResetInteractivity());

            _customToolBar.LayerChanged.Subscribe(layer => _map.SetWorldMapLayer(layer));
        }

        private void ResetInteractivity()
        {
            //if (_isDirtyDecorator == true)
            //{
            //    _userLayerSource.EditFeature(_currentFeature.Copy());

            //    _isDirtyDecorator = false;
            //}

            Tip = null;

            //_currentFeature = null;

            //RemoveInteractiveLayer();

            ActualController = new DefaultController();
        }

        private void MapListener_LeftClickOnMap(object? sender, EventArgs e)
        {
            if (sender is MapInfo mapInfo)
            {
                //var feature = mapInfo.Feature;

                //IDecorator? decorator = null;

                //if (_customToolBar.SelectGeometry.IsCheck == true)
                //{
                //    RemoveInteractiveLayer();

                //    if (feature is GeometryFeature gf && gf != _currentFeature)
                //    {
                //        CreateInteractiveSelectLayer(mapInfo.Layer, mapInfo.Feature);

                //        _currentFeature = gf;
                //    }
                //    else
                //    {
                //        _currentFeature = null;
                //    }

                //    return;
                //}
                //else if (_customToolBar.ScaleGeometry.IsCheck == true)
                //{
                //    if (feature is GeometryFeature gf && gf.Geometry is not Point)
                //    {
                //        decorator = new ScaleDecorator(gf);
                //    }
                //}
                //else if (_customToolBar.TranslateGeometry.IsCheck == true)
                //{
                //    if (feature is GeometryFeature gf)
                //    {
                //        decorator = new TranslateDecorator(gf);
                //    }
                //}
                //else if (_customToolBar.RotateGeometry.IsCheck == true)
                //{
                //    if (feature is GeometryFeature gf && gf.Geometry is not Point)
                //    {
                //        decorator = new RotateDecorator(gf);
                //    }
                //}
                //else if (_customToolBar.EditGeometry.IsCheck == true)
                //{
                //    if (feature is GeometryFeature gf && gf.Geometry is not Point)
                //    {
                //        decorator = new EditDecorator(gf);
                //    }
                //}

                //if (decorator == null)
                //{
                //    return;
                //}

                //RemoveInteractiveLayer();

                //if (feature is GeometryFeature gf2 && gf2 != _currentFeature)
                //{
                //    CreateInteractiveLayer(mapInfo.Layer, decorator);

                //    MapObserver = new MapObserver(decorator);

                //    _currentFeature = gf2;

                //    _isDirtyDecorator = true;
                //}
                //else
                //{
                //    ToolBar.Uncheck();
                //}
            }
            else if (sender is string name)
            {
                if (_footprintObserver.IsActive == true)
                {
                    _footprintObserver.SelectFootprintInfo(name);
                }
            }
        }

        public event EventHandler? AOIChanged;

        private void Map_DataChanged(object sender, Mapsui.Fetcher.DataChangedEventArgs e)
        {
            var list = new List<MapLayer>();

            if (Map != null)
            {
                foreach (var layer in Map.Layers)
                {
                    //   string crs = string.Empty;
                    string format = string.Empty;

                    if (layer is TileLayer tileLayer)
                    {
                        // crs = tileLayer.TileSource.Schema.Srs;
                        format = tileLayer.TileSource.Schema.Format;
                    }

                    // if (string.IsNullOrEmpty(crs) == true)
                    {
                        //     crs = layer.CRS;
                    }

                    list.Add(new MapLayer()
                    {
                        Name = layer.Name,
                        //     CRS = crs,
                        Format = format,
                    });
                }
            }

            MapLayers = new ObservableCollection<MapLayer>(list);
        }

        private string FeatureAreaEndCreating(GeometryFeature feature)
        {
            var bb = feature.Extent;//Geometry.BoundingBox;
            var coord = SphericalMercator.ToLonLat(bb.Centroid.X, bb.Centroid.Y).ToMPoint();
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y).ToMPoint()).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);
            return $"{FormatHelper.ToArea(area)} | {FormatHelper.ToCoordinate(coord.X, coord.Y)}";
        }

        private void ZoomInCommand()
        {
            if (Map == null)
            {
                return;
            }

            Map.Initialized = false;
            Map.Home = (n) => n.ZoomIn();

            // HACK: add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
            var layer = new Mapsui.Layers.Layer();
            Map.Layers.Add(layer);
            Map.Layers.Remove(layer);
        }

        private void ZoomOutCommand()
        {
            if (Map == null)
            {
                return;
            }

            Map.Initialized = false;
            Map.Home = (n) => n.ZoomOut();

            var layer = new Mapsui.Layers.Layer();
            Map.Layers.Add(layer);
            Map.Layers.Remove(layer);
        }

        private void RectangleCommand()
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            if (editLayer == null)
            {
                return;
            }

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

                editLayer.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIRectangle.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void PolygonCommand()
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            if (editLayer == null)
            {
                return;
            }

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

                editLayer.AddAOI(new InteractivePolygon(feature), Styles.FeatureType.AOIPolygon.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать полигон" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void CircleCommand()
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            if (editLayer == null)
            {
                return;
            }

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

                editLayer.AddAOI(new InteractiveCircle(feature), Styles.FeatureType.AOICircle.ToString());

                Tip = null;

                InfoPanel.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать круг" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private void RouteCommand()
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            if (editLayer == null)
            {
                return;
            }

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

                editLayer.AddRoute(new InteractiveRoute(feature), Styles.FeatureType.Route.ToString());

                Tip = null;

                _customToolBar.Uncheck();
            };

            editLayer.ClearRoute();

            InfoPanel.CloseAll(typeof(RouteInfoPanel));

            Tip = new Tip() { Text = "Кликните, чтобы начать измерение" };

            Behavior = new InteractiveBehavior(designer);

            ActualController = new DrawingController();
        }

        private AOIInfoPanel CreateAOIPanel(GeometryFeature feature)
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            var descr = FeatureAreaEndCreating(feature);

            var panel = new AOIInfoPanel()
            {
                Text = descr,
            };

            panel.Close.Subscribe(_ =>
            {
                if (editLayer != null)
                {
                    editLayer.ResetAOI();
                    editLayer.DataHasChanged();
                }

                AOIChanged?.Invoke(null, EventArgs.Empty);

                Tip = null;

                ToolBar.Uncheck();
            });

            return panel;
        }

        private RouteInfoPanel CreateRoutePanel(IRouteDesigner designer)
        {
            var editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            var distance = designer.Distance();

            var panel = new RouteInfoPanel()
            {
                Text = FormatHelper.ToDistance(distance),
            };

            panel.Close.Subscribe(_ =>
            {
                if (editLayer != null)
                {
                    editLayer.ClearRoute();
                    editLayer.DataHasChanged();
                }

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

        public CustomToolBar ToolBar => _customToolBar;

        [Reactive]
        public ObservableCollection<MapLayer>? MapLayers { get; set; }

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
