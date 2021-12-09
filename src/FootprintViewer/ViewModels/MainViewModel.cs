using FootprintViewer.Data;
using FootprintViewer.Graphics;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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

        public MainViewModel()
        {
            this.WhenAnyValue(s => s.Map).Subscribe(_ => MapChanged());

            this.WhenAnyValue(s => s.UserDataSource).Subscribe(_ => UserDataSourceChanged());

            ActualController = new EditController();

            ToolManager = CreateToolManager();
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

                ToolManager.WorldMapSelector = WorldMapSelector;
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

        private void CloseInfoPanelAOI()
        {
            _editLayer.ResetAOI();
            _editLayer.DataHasChanged();

            AOIChanged?.Invoke(null, EventArgs.Empty);

            ToolManager.ResetAllTools();
        }

        private void CloseInfoPanelRoute()
        {
            _editLayer.ClearRoute();
            _editLayer.DataHasChanged();

            ToolManager.ResetAllTools();
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

            Tip = new Tip()
            {
                Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник",
            };

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

                var descr = FeatureAreaEndCreating(feature);

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, CloseInfoPanelAOI);

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);

                ToolManager.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var area = GetFeatureArea((Feature)e.AddInfo.Feature);
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                Tip.Text = "Отпустите клавишу мыши для завершения рисования";

                _editLayer.DataHasChanged();
            };


            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                var descr = FeatureAreaEndCreating(feature);

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, CloseInfoPanelAOI);

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);
            };

            ActualController = new DrawRectangleController();
        }

        private void PolygonCommand()
        {
            Plotter = new Plotter(InteractivePolygon.Build());

            Tip = new Tip()
            {
                Text = "Нажмите и перетащите, чтобы нарисовать полигон",
            };

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
                    Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                    Tip.Text = "Щелкните по первой точке, чтобы закрыть эту фигуру";
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

                var descr = FeatureAreaEndCreating((Feature)e.AddInfo.Feature);

                AOIChanged?.Invoke(e.AddInfo.Feature.Geometry, EventArgs.Empty);

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, CloseInfoPanelAOI);

                ToolManager.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                _editLayer.DataHasChanged();
            };

            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                var descr = FeatureAreaEndCreating(feature);

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, CloseInfoPanelAOI);

                AOIChanged?.Invoke(feature.Geometry, EventArgs.Empty);
            };

            ActualController = new DrawPolygonController();
        }

        private void CircleCommand()
        {
            if (Map == null)
            {
                return;
            }

            Plotter = new Plotter(InteractiveCircle.Build());

            Tip = new Tip()
            {
                Text = "Нажмите и перетащите, чтобы нарисовать круг",
            };

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

                var descr = FeatureAreaEndCreating((Feature)e.AddInfo.Feature);

                AOIChanged?.Invoke(e.AddInfo.Feature.Geometry, EventArgs.Empty);

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, CloseInfoPanelAOI);

                ToolManager.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var area = GetFeatureArea((Feature)e.AddInfo.Feature);
                Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                Tip.Text = "Отпустите клавишу мыши для завершения рисования";

                _editLayer.DataHasChanged();
            };

            Plotter.EndEditing += (s, e) =>
            {
                var feature = (Feature)e.Feature;

                var descr = FeatureAreaEndCreating(feature);

                void Closing()
                {
                    _editLayer.ResetAOI();
                    _editLayer.DataHasChanged();

                    AOIChanged?.Invoke(null, EventArgs.Empty);
                }

                InfoPanel?.Open(nameof(InfoPanelType.AOI), descr, Closing);

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

            InfoPanel?.Close(nameof(InfoPanelType.Route));

            Plotter = new Plotter(InteractiveRoute.Build());

            Tip = new Tip()
            {
                Text = "Кликните, чтобы начать измерение",
            };

            Plotter.BeginCreating += (s, e) =>
            {
                var distance = GetRouteLength(e.AddInfo);
                Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";
                Tip.Text = "";

                _editLayer.AddRoute(e.AddInfo);
                _editLayer.DataHasChanged();
            };

            Plotter.Creating += (s, e) =>
            {
                var distance = GetRouteLength(e.AddInfo);

                InfoPanel?.Open(nameof(InfoPanelType.Route), FormatHelper.ToDistance(distance), CloseInfoPanelRoute);

                _editLayer.DataHasChanged();
            };

            Plotter.EndCreating += (s, e) =>
            {
                _editLayer.ClearRouteHelpers();
                _editLayer.DataHasChanged();

                Tip = null;

                ToolManager.ResetAllTools();
            };

            Plotter.Hover += (s, e) =>
            {
                var distance = GetRouteLength(e.AddInfo);
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

        private ToolManager CreateToolManager()
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

            var toolManager = new ToolManager();

            toolManager.ZoomIn = toolZoomIn;
            toolManager.ZoomOut = toolZoomOut;
            toolManager.AOICollection = aoiCollection;
            toolManager.RouteDistance = toolRouteDistance;
            toolManager.Edit = toolEdit;
            toolManager.WorldMaps = toolWorldMaps;

            return toolManager;
        }

        [Reactive]
        public IUserDataSource? UserDataSource { get; set; }

        [Reactive]
        public IDataSource? DataSource { get; set; }

        [Reactive]
        public Map? Map { get; set; }

        [Reactive]
        public SidePanel? SidePanel { get; set; }

        [Reactive]
        public ObservableCollection<MapLayer>? MapLayers { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Plotter? Plotter { get; set; }

        [Reactive]
        public ToolManager ToolManager { get; set; }

        [Reactive]
        public InfoPanel? InfoPanel { get; set; }

        [Reactive]
        public Tip? Tip { get; set; }

        [Reactive]
        public WorldMapSelector? WorldMapSelector { get; set; }
    }

    public class MapLayer
    {
        public string? Name { get; set; }

        public string? CRS { get; set; }

        public string? Format { get; set; }
    }
}
