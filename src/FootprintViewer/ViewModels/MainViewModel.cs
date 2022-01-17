using FootprintViewer.Data;
using FootprintViewer.Interactivity;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
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
        private enum InfoPanelType { AOI, Route }

        private EditLayer _editLayer = new EditLayer();

        private readonly Map? _map;
        private readonly UserDataSource? _userDataSource;
        private readonly IDataSource? _dataSource;
        private readonly InfoPanel? _infoPanel;
        private readonly SidePanel? _sidePanel;
        private readonly ProjectFactory? _factory;

        public MainViewModel()
        {
            _dataSource = Locator.Current.GetService<IDataSource>();
            _factory = Locator.Current.GetService<ProjectFactory>();
            _userDataSource = Locator.Current.GetService<UserDataSource>();
            _sidePanel = Locator.Current.GetService<SidePanel>();     
            _map = Locator.Current.GetService<Map>();

            ActualController = new EditController();

            _infoPanel = _factory?.CreateInfoPanel();

            ToolBar = CreateToolBar();

            this.WhenAnyValue(s => s.Map).Subscribe(_ => MapChanged());

            this.WhenAnyValue(s => s.UserDataSource).Subscribe(_ => UserDataSourceChanged());
        }

        public event EventHandler? AOIChanged;

        private void MapChanged()
        {
            if (Map != null)
            {
                _editLayer = Map.GetLayer<EditLayer>(LayerType.Edit);

                Map.DataChanged += Map_DataChanged;
            }
        }

        private void UserDataSourceChanged()
        {
            if (UserDataSource != null)
            {
                WorldMapSelector = new WorldMapSelector(UserDataSource.WorldMapSources);

                WorldMapSelector.SelectLayer += (layer) => { Map?.SetWorldMapLayer(layer); };

                ToolBar.WorldMapSelector = WorldMapSelector;
            }
        }

        private void Map_DataChanged(object sender, Mapsui.Fetcher.DataChangedEventArgs e)
        {
            var list = new List<MapLayer>();

            if (Map != null)
            {
                foreach (var layer in Map.Layers)
                {
                    string crs = string.Empty;
                    string format = string.Empty;

                    if (layer is TileLayer tileLayer)
                    {
                        crs = tileLayer.TileSource.Schema.Srs;
                        format = tileLayer.TileSource.Schema.Format;
                    }

                    if (string.IsNullOrEmpty(crs) == true)
                    {
                        crs = layer.CRS;
                    }

                    list.Add(new MapLayer()
                    {
                        Name = layer.Name,
                        CRS = crs,
                        Format = format,
                    });
                }
            }

            MapLayers = new ObservableCollection<MapLayer>(list);
        }

        private string FeatureAreaEndCreating(Feature feature)
        {
            var bb = feature.Geometry.BoundingBox;
            var coord = SphericalMercator.ToLonLat(bb.Centroid.X, bb.Centroid.Y);
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);
            return $"{FormatHelper.ToArea(area)} | {FormatHelper.ToCoordinate(coord.X, coord.Y)}";
        }

        private double GetFeatureArea(Feature feature)
        {
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            return Math.Abs(area);
        }

        private double GetRouteLength(AddInfo addInfo)
        {
            var geometry = (LineString)addInfo.Feature.Geometry;
            var fHelp = addInfo.HelpFeatures.Single();
            var verts0 = geometry.AllVertices();
            var verts1 = fHelp.Geometry.AllVertices();
            var verts = verts0.Union(verts1);
            var vertices = verts.Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            return SphericalUtil.ComputeDistance(vertices);
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
            Plotter = new Plotter(InteractiveRectangle.Build());

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник" };

            Plotter.BeginCreating += (s, e) =>
            {
                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();
            };

            Plotter.EndCreating += (s, e) =>
            {
                var feature = (Feature)e.AddInfo.Feature;

                _editLayer.ResetAOI();
                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();

                Tip = null;

                InfoPanel?.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                ToolBar.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var area = GetFeatureArea((Feature)e.AddInfo.Feature);

                Tip.Text = "Отпустите клавишу мыши для завершения рисования";
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";

                _editLayer.DataHasChanged();
            };


            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                InfoPanel?.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);
            };

            ActualController = new DrawRectangleController();
        }

        private void PolygonCommand()
        {
            Plotter = new Plotter(InteractivePolygon.Build());

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать полигон" };

            Plotter.BeginCreating += (s, e) =>
            {
                Tip.Text = "Нажмите, чтобы продолжить рисование фигуры";

                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();
            };

            Plotter.Creating += (s, e) =>
            {
                if (e.AddInfo.Feature.Geometry.AllVertices().Count() > 2)
                {
                    var area = GetFeatureArea((Feature)e.AddInfo.Feature);

                    Tip.Text = "Щелкните по первой точке, чтобы закрыть эту фигуру";
                    Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                }

                _editLayer.DataHasChanged();
            };

            Plotter.EndCreating += (s, e) =>
            {
                _editLayer.ClearAOIHelpers();

                _editLayer.ResetAOI();
                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();

                Tip = null;

                var feature = (Feature)e.AddInfo.Feature;

                AOIChanged?.Invoke(e.AddInfo.Feature.Geometry, EventArgs.Empty);

                InfoPanel?.Show(CreateAOIPanel(feature));

                ToolBar.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                _editLayer.DataHasChanged();
            };

            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                InfoPanel?.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);
            };

            ActualController = new DrawPolygonController();
        }

        private AOIInfoPanel CreateAOIPanel(Feature feature)
        {
            var descr = FeatureAreaEndCreating(feature);

            var panel = new AOIInfoPanel()
            {
                Text = descr,
            };

            panel.Close.Subscribe(_ =>
            {
                _editLayer.ResetAOI();
                _editLayer.DataHasChanged();

                AOIChanged?.Invoke(null, EventArgs.Empty);

                ToolBar.ResetAllTools();
            });

            return panel;
        }

        private RouteInfoPanel CreateRoutePanel(AddInfo addInfo)
        {
            var distance = GetRouteLength(addInfo);

            var panel = new RouteInfoPanel()
            {
                Text = FormatHelper.ToDistance(distance),
            };

            panel.Close.Subscribe(_ =>
            {
                _editLayer.ClearRoute();
                _editLayer.DataHasChanged();

                ToolBar.ResetAllTools();
            });

            return panel;
        }

        private void CircleCommand()
        {
            if (Map == null)
            {
                return;
            }

            Plotter = new Plotter(InteractiveCircle.Build());

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать круг" };

            Plotter.BeginCreating += (s, e) =>
            {
                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();
            };

            Plotter.EndCreating += (s, e) =>
            {
                _editLayer.ResetAOI();
                _editLayer.AddAOI(e.AddInfo);
                _editLayer.DataHasChanged();

                Tip = null;

                var feature = (Feature)e.AddInfo.Feature;

                AOIChanged?.Invoke(e.AddInfo.Feature.Geometry, EventArgs.Empty);

                InfoPanel?.Show(CreateAOIPanel(feature));

                ToolBar.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var area = GetFeatureArea((Feature)e.AddInfo.Feature);

                Tip.Text = "Отпустите клавишу мыши для завершения рисования";
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";

                _editLayer.DataHasChanged();
            };

            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                InfoPanel?.Show(CreateAOIPanel(feature));

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);
            };

            ActualController = new DrawCircleController();
        }

        private void RouteDistanceCommand()
        {
            if (Map == null)
            {
                return;
            }

            _editLayer.ClearRoute();

            InfoPanel?.CloseAll(typeof(RouteInfoPanel));

            Plotter = new Plotter(InteractiveRoute.Build());

            Tip = new Tip() { Text = "Кликните, чтобы начать измерение" };

            Plotter.BeginCreating += (s, e) =>
            {
                var distance = GetRouteLength(e.AddInfo);

                Tip.Text = "";
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";

                _editLayer.AddRoute(e.AddInfo);
                _editLayer.DataHasChanged();
            };

            Plotter.Creating += (s, e) =>
            {
                InfoPanel?.Show(CreateRoutePanel(e.AddInfo));

                _editLayer.DataHasChanged();
            };

            Plotter.EndCreating += (s, e) =>
            {
                _editLayer.ClearRouteHelpers();
                _editLayer.DataHasChanged();

                Tip = null;

                ToolBar.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var distance = GetRouteLength(e.AddInfo);

                Tip.Text = "";
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";

                _editLayer.DataHasChanged();
            };

            ActualController = new DrawRouteController();
        }

        private void EditCommand()
        {
            Tip = null;
            ActualController = new EditController();
        }

        private void WorldMapCommand()
        {
            WorldMapSelector?.Click();
        }

        private ToolBar CreateToolBar()
        {
            var toolZoomIn = new Tool()
            {
                Title = "+",
                Tooltip = "Приблизить",
                Command = ReactiveCommand.Create(ZoomInCommand),
            };

            var toolZoomOut = new Tool()
            {
                Title = "-",
                Tooltip = "Отдалить",
                Command = ReactiveCommand.Create(ZoomOutCommand),
            };

            var toolRectangle = new Tool()
            {
                Title = "AddRectangle",
                Tooltip = "Нарисуйте прямоугольную AOI",
                Command = ReactiveCommand.Create(RectangleCommand),
            };

            var toolPolygon = new Tool()
            {
                Title = "AddPolygon",
                Tooltip = "Нарисуйте полигональную AOI",
                Command = ReactiveCommand.Create(PolygonCommand),
            };

            var toolCircle = new Tool()
            {
                Title = "AddCircle",
                Tooltip = "Нарисуйте круговую AOI",
                Command = ReactiveCommand.Create(CircleCommand),
            };

            var aoiCollection = new ToolCollection(new[] { toolRectangle, toolPolygon, toolCircle });

            var toolRouteDistance = new Tool()
            {
                Title = "Route",
                Tooltip = "Измерить расстояние",
                Command = ReactiveCommand.Create(RouteDistanceCommand),
            };

            var toolEdit = new Tool()
            {
                Title = "Edit",
                Command = ReactiveCommand.Create(EditCommand),
            };

            var toolWorldMaps = new Tool()
            {
                Title = "WorldMaps",
                Tooltip = "Список слоев",
                Command = ReactiveCommand.Create(WorldMapCommand),
            };

            var toolBar = new ToolBar();

            toolBar.ZoomIn = toolZoomIn;
            toolBar.ZoomOut = toolZoomOut;
            toolBar.AOICollection = aoiCollection;
            toolBar.RouteDistance = toolRouteDistance;
            toolBar.Edit = toolEdit;
            toolBar.WorldMaps = toolWorldMaps;

            return toolBar;
        }
 
        public IUserDataSource? UserDataSource => _userDataSource;
  
        public IDataSource? DataSource => _dataSource;

        public Map? Map => _map;

        public SidePanel? SidePanel => _sidePanel;

        [Reactive]
        public ObservableCollection<MapLayer>? MapLayers { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Plotter? Plotter { get; set; }

        [Reactive]
        public ToolBar ToolBar { get; set; }
  
        public InfoPanel? InfoPanel => _infoPanel;

        [Reactive]
        public Tip? Tip { get; set; }

        [Reactive]
        public WorldMapSelector? WorldMapSelector { get; set; }

        public MapListener? MapListener { get; set; }
    }

    public class MapLayer
    {
        public string? Name { get; set; }

        public string? CRS { get; set; }

        public string? Format { get; set; }
    }
}
