using FootprintViewer.InteractivityEx;
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

        private readonly EditLayer _editLayer = new EditLayer();

        private readonly Map _map;
        private readonly InfoPanel _infoPanel;
        private readonly SidePanel _sidePanel;
        private readonly ProjectFactory _factory;
        private readonly ToolBar _toolBar;

        public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _factory = dependencyResolver.GetExistingService<ProjectFactory>();
            _map = dependencyResolver.GetExistingService<Map>();
            _sidePanel = dependencyResolver.GetExistingService<SidePanel>();
            _toolBar = dependencyResolver.GetExistingService<ToolBar>();

            ActualController = new EditController();

            _infoPanel = _factory.CreateInfoPanel();

            _toolBar.ZoomInClick.Subscribe(_ => ZoomInCommand());
            _toolBar.ZoomOutClick.Subscribe(_ => ZoomOutCommand());
            _toolBar.RectangleClick.Subscribe(_ => RectangleCommand());
            _toolBar.PolygonClick.Subscribe(_ => PolygonCommand());
            _toolBar.CircleClick.Subscribe(_ => CircleCommand());
            _toolBar.RouteDistanceClick.Subscribe(_ => RouteDistanceCommand());
            _toolBar.EditClick.Subscribe(_ => EditCommand());
            _toolBar.LayerChanged.Subscribe(layer => _map.SetWorldMapLayer(layer));

            _editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            _map.DataChanged += Map_DataChanged;
        }

        public event EventHandler? AOIChanged;

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

        public Map Map => _map;

        public SidePanel SidePanel => _sidePanel;

        public InfoPanel InfoPanel => _infoPanel;

        public MapListener? MapListener { get; set; }

        public ToolBar ToolBar => _toolBar;

        [Reactive]
        public ObservableCollection<MapLayer>? MapLayers { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Plotter? Plotter { get; set; }

        [Reactive]
        public Tip? Tip { get; set; }
    }

    public class MapLayer
    {
        public string? Name { get; set; }

        public string? CRS { get; set; }

        public string? Format { get; set; }
    }
}
