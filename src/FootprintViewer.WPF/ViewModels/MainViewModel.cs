using BruTile.Wms;
using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Graphics;
using FootprintViewer.Models;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Operation.Distance;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Windows.Input;

namespace FootprintViewer.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private enum InfoPanelType { AOI, Route }

        public MainViewModel()
        {

            ActualController = new EditController();
            
            Map = SampleBuilder.CreateMap();

            var editLayer = (EditLayer)Map.Layers.First(l => l.Name == nameof(LayerType.EditLayer));

            Map.DataChanged += Map_DataChanged;

            var tab = new SceneSearch() 
            {
                Title = "Поиск сцены",
                Name = "Scene",
                Map = Map,     
                DataSource = new DataSource(),
            };

            tab.Filter.FromDate = DateTime.Today.AddDays(-1);
            tab.Filter.ToDate = DateTime.Today.AddDays(1);

            SidePanel = new SidePanel() { Tabs = new ObservableCollection<SidePanelTab>(new[] { tab }), SelectedTab = tab  };

            ToolManager = CreateToolManager();

            InfoPanel = SampleBuilder.CreateInfoPanel();
        }



        private void Map_DataChanged(object sender, Mapsui.Fetcher.DataChangedEventArgs e)
        {
            var list = new List<MapLayer>();

            foreach (var layer in Map.Layers)
            {
                string crs = default;
                string format = default;
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

        private ToolManager CreateToolManager()
        {
            var toolZoomIn = new Tool()
            {
                Title = "+",
                Tooltip = "Приблизить",
                Command = new RelayCommand(_ =>
                {         
                    Map.Initialized = false;
                    Map.Home = (n) => n.ZoomIn();

                    // HACK: add/remove layer for calling method CallHomeIfNeeded() and new initializing with Home
                    var layer = new Mapsui.Layers.Layer();
                    Map.Layers.Add(layer);
                    Map.Layers.Remove(layer);
                }),
            };

            var toolZoomOut = new Tool()
            {
                Title = "-",
                Tooltip = "Отдалить",
                Command = new RelayCommand(_ => 
                {              
                    Map.Initialized = false;
                    Map.Home = (n) => n.ZoomOut();

                    var layer = new Mapsui.Layers.Layer();
                    Map.Layers.Add(layer);
                    Map.Layers.Remove(layer);
                }),
            };

            var toolRectangle = new Tool()
            {
                Title = "Rect",
                Tooltip = "Нарисуйте прямоугольную AOI",
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));

                    var tab = (SceneSearch)SidePanel.Tabs.Single();

                    Plotter = new Plotter(InteractiveRectangle.Build());

                    Tip = new Tip()
                    {
                        Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник",
                    };

                    Plotter.BeginCreating += (s, e) => 
                    {                
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();                     
                    };

                    Plotter.EndCreating += (s, e) =>
                    {
                        var feature = (Feature)e.AddInfo.Feature;

                        layer.ResetAOI();
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();

                        Tip = null;

                        var descr = FeatureAreaEndCreating(feature);
                        
                        void Closing()
                        {                           
                            layer.ResetAOI();
                            layer.DataHasChanged();

                            tab.ResetAOI();
                        }
                                                
                        InfoPanel.Open(nameof(InfoPanelType.AOI), descr, Closing);

                        tab.SetAOI(feature);

                        ToolManager.ResetAllTools();
                    };
                    
                    Plotter.Hover += (s, e) =>
                    {
                        var area = GetFeatureArea((Feature)e.AddInfo.Feature);
                        Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                        Tip.Text = "Отпустите клавишу мыши для завершения рисования";

                        layer.DataHasChanged();
                    };

                    ActualController = new DrawRectangleController();                 
                }),
            };

            var toolPolygon = new Tool()
            {
                Title = "Poly",
                Tooltip = "Нарисуйте полигональную AOI",
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));

                    Plotter = new Plotter(InteractivePolygon.Build());

                    Tip = new Tip()
                    {
                        Text = "Нажмите и перетащите, чтобы нарисовать полигон",
                    };

                    Plotter.BeginCreating += (s, e) =>
                    {
                        Tip.Text = "Нажмите, чтобы продолжить рисование фигуры";

                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();
                    };

                    Plotter.Creating += (s, e) =>
                    {
                        if (e.AddInfo.Feature.Geometry.AllVertices().Count() > 2)
                        {
                            var area = GetFeatureArea((Feature)e.AddInfo.Feature);
                            Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                            Tip.Text = "Щелкните по первой точке, чтобы закрыть эту фигуру";
                        }

                        layer.DataHasChanged();
                    };

                    Plotter.EndCreating += (s, e) =>
                    {
                        layer.ClearAOIHelpers();

                        layer.ResetAOI();
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();

                        Tip = null;

                        var descr = FeatureAreaEndCreating((Feature)e.AddInfo.Feature);

                        void Closing()
                        {
                            layer.ResetAOI();
                            layer.DataHasChanged();
                        }

                        InfoPanel.Open(nameof(InfoPanelType.AOI), descr, Closing);

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) => 
                    {
                        layer.DataHasChanged(); 
                    };


                    ActualController = new DrawPolygonController(); 
                }),
            };

            var toolCircle = new Tool()
            {
                Title = "Circle",
                Tooltip = "Нарисуйте круговую AOI",
                Command = new RelayCommand(_ =>
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));

                    Plotter = new Plotter(InteractiveCircle.Build());

                    Tip = new Tip()
                    {
                        Text = "Нажмите и перетащите, чтобы нарисовать круг",
                    };

                    Plotter.BeginCreating += (s, e) =>
                    {
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();
                    };

                    Plotter.EndCreating += (s, e) =>
                    {
                        layer.ResetAOI();
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();

                        Tip = null;

                        var descr = FeatureAreaEndCreating((Feature)e.AddInfo.Feature);

                        void Closing()
                        {
                            layer.ResetAOI();
                            layer.DataHasChanged();
                        }

                        InfoPanel.Open(nameof(InfoPanelType.AOI), descr, Closing);

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) =>
                    {
                        var area = GetFeatureArea((Feature)e.AddInfo.Feature);
                        Tip.Title = $"Область: {FormatHelper.ToArea(area)}";
                        Tip.Text = "Отпустите клавишу мыши для завершения рисования";

                        layer.DataHasChanged(); 
                    };

                    ActualController = new DrawCircleController(); 
                }),
            };
      
            var aoiCollection = new ToolCollection(new[] { toolRectangle, toolPolygon, toolCircle });
        
            var toolRouteDistance = new Tool()
            {
                Title = "Route",
                Tooltip = "Измерить расстояние",
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));
                        
                    layer.ClearRoute();

                    InfoPanel.Close(nameof(InfoPanelType.Route));

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

                        layer.AddRoute(e.AddInfo);
                        layer.DataHasChanged();
                    };

                    Plotter.Creating += (s, e) =>
                    {
                        var distance = GetRouteLength(e.AddInfo);

                        void Closing()
                        {
                            layer.ClearRoute();
                            layer.DataHasChanged();
                        }

                        InfoPanel.Open(nameof(InfoPanelType.Route), FormatHelper.ToDistance(distance), Closing);
                        
                        layer.DataHasChanged();
                    };

                    Plotter.EndCreating += (s, e) =>
                    {
                        layer.ClearRouteHelpers();
                        layer.DataHasChanged();

                        Tip = null;

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) => 
                    {
                        var distance = GetRouteLength(e.AddInfo);                   
                        Tip.Title = $"Расстояние: {FormatHelper.ToDistance(distance)}";

                        layer.DataHasChanged();
                    };

                    ActualController = new DrawRouteController(); 
                })
            };

            var toolEdit = new Tool()
            {
                Title = "Edit",
                Command = new RelayCommand(_ => ActualController = new EditController())
            };

            var toolManager = new ToolManager();

            toolManager.ZoomIn = toolZoomIn;
            toolManager.ZoomOut = toolZoomOut;
            toolManager.AOICollection = aoiCollection;
            toolManager.RouteDistance = toolRouteDistance;
            toolManager.Edit = toolEdit;

            return toolManager;
        }



        [Reactive]
        public Map Map { get; set; }

        [Reactive]
        public SidePanel SidePanel { get; set; }

        [Reactive]
        public ObservableCollection<MapLayer> MapLayers { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Plotter Plotter { get; set; }

        [Reactive]
        public ToolManager ToolManager { get; set; }

        [Reactive]
        public InfoPanel InfoPanel { get; set; }

        [Reactive]
        public Tip? Tip { get; set; }
    }

    public class MapLayer
    {
        public string Name { get; set; }

        public string CRS { get; set; }

        public string Format { get; set; }
    }
}
