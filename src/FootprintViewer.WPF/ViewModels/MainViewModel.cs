using BruTile.Wms;
using FootprintViewer.Graphics;
using FootprintViewer.Models;
using FootprintViewer.ViewModels;
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
        public event EventHandler CurrentFootprint;
        
        public MainViewModel()
        {
            Footprints = new ObservableCollection<Footprint>(ResourceManager.GetFootprints());

            ActualController = new EditController();
            
            Map = SampleBuilder.CreateMap();

            var editLayer = (EditLayer)Map.Layers.First(l => l.Name == nameof(LayerType.EditLayer));

       //     Plotter = new Plotter(/*editLayer*/);

       //     Plotter.EndCreating += FeatureEndCreating;

       //     Plotter.HoverCreating += FeatureHoverCreating;

        //    Plotter.StepCreating += FeatureStepCreating;

            Map.DataChanged += Map_DataChanged;

            this.WhenAnyValue(s => s.SelectedFootprint).Subscribe(footprint =>
            {
                if (footprint != null)
                {
                    var layer = MapsuiHelper.CreateMbTilesLayer(footprint.Path);

                    Map.Layers.Replace(nameof(LayerType.FootprintLayer), layer);

                    CurrentFootprint?.Invoke(this, EventArgs.Empty);
                }
            });

            MouseOverEnterCommand = ReactiveCommand.Create<Footprint>(ShowFootprintBorder);

            MouseOverLeaveCommand = ReactiveCommand.Create(HideFootprintBorder);

            ToolManager = CreateToolManager();

            InfoPanel = SampleBuilder.CreateInfoPanel();
        }

        public ReactiveCommand<Footprint, Unit> MouseOverEnterCommand { get; }

        public ReactiveCommand<Unit, Unit> MouseOverLeaveCommand { get; }

        private void ShowFootprintBorder(Footprint footprint)
        {
            var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

            if (layers != null)
            {
                var layer = layers.SingleOrDefault();

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.Add(new Feature() { Geometry = footprint.Geometry });
                    writableLayer.DataHasChanged();
                }
            }
        }

        private void HideFootprintBorder()
        {
            var layers = Map.Layers.FindLayer(nameof(LayerType.FootprintBorderLayer));

            if (layers != null)
            {
                var layer = layers.SingleOrDefault();

                if (layer != null && layer is WritableLayer writableLayer)
                {
                    writableLayer.Clear();
                    writableLayer.DataHasChanged();
                }
            }
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

        //private void FeatureEndCreating(object? sender, FeatureEventArgs e)
        //{
        //    var feature = e.Feature;
        //    var bb = feature.Geometry.BoundingBox;
        //    var coord = ProjectHelper.ToString(bb.Centroid);
        //    var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
        //    var area = SphericalUtil.ComputeSignedArea(vertices);
        //    area = Math.Abs(area);

        //    AOIDescription = $"{area:N2} km² | {coord}";
        //    //RouteDescription;
        //}

        //private void FeatureHoverCreating(object? sender, FeatureEventArgs e)
        //{
        //    var feature = e.Feature;
        //    var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
        //    var area = SphericalUtil.ComputeSignedArea(vertices);
        //    area = Math.Abs(area);

        //    AOIHoverDescription = $"{area:N2} km²";
        //    //RouteHoverDescription;
        //}

        //private void FeatureStepCreating(object? sender, FeatureEventArgs e)
        //{
        //    var feature = e.Feature;

        //    if (feature["Name"].Equals(FeatureType.Route.ToString()) == true)
        //    {
        //        var geometry = (LineString)feature.Geometry;
        //        var vertices = geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
        //        var distance = SphericalUtil.ComputeDistance(vertices);

        //        RouteDescription = $"{distance:N2} km";
        //    }
        //}

        private string FeatureLengthStepCreating(Feature feature)
        {         
            if (feature["Name"].Equals(FeatureType.Route.ToString()) == true)
            {
                var geometry = (LineString)feature.Geometry;
                var vertices = geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
                var distance = SphericalUtil.ComputeDistance(vertices);

                return $"{distance:N2} km";
            }

            return "error";
        }

        private string FeatureAreaEndCreating(Feature feature)
        {          
            var bb = feature.Geometry.BoundingBox;
            var coord = ProjectHelper.ToString(bb.Centroid);
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);
            return $"{area:N2} km² | {coord}";        
        }

        private ToolManager CreateToolManager()
        {
            var toolZoomIn = new Tool()
            {
                Title = "+",
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
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));
               
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
                                                
                        InfoPanel.OpenAOI(descr, Closing);

                        ToolManager.ResetAllTools();
                    };
                    
                    Plotter.Hover += (s, e) =>
                    {
                        if (Tip != null)
                        {
                            var vertices = e.AddInfo.Feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
                            var area = SphericalUtil.ComputeSignedArea(vertices);
                            area = Math.Abs(area);
                            Tip.Title = $"Область: {area:N2} km²";
                        }

                        layer.DataHasChanged();
                    };

                    ActualController = new DrawRectangleController();                 
                }),
            };

            var toolPolygon = new Tool()
            {
                Title = "Poly",
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));

                    Plotter = new Plotter(InteractivePolygon.Build());

                    Plotter.BeginCreating += (s, e) =>
                    {
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();
                    };

                    Plotter.Creating += (s, e) => layer.DataHasChanged();

                    Plotter.EndCreating += (s, e) =>
                    {
                        layer.ClearAOIHelpers();

                        layer.ResetAOI();
                        layer.AddAOI(e.AddInfo);
                        layer.DataHasChanged();

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) => layer.DataHasChanged();


                    ActualController = new DrawPolygonController(); 
                }),
            };

            var toolCircle = new Tool()
            {
                Title = "Circle",
                Command = new RelayCommand(_ =>
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));

                    Plotter = new Plotter(InteractiveCircle.Build());

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

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) => { layer.DataHasChanged(); };

                    ActualController = new DrawCircleController(); 
                }),
            };
      
            var aoiCollection = new ToolCollection(new[] { toolRectangle, toolPolygon, toolCircle });
        
            var toolRouteDistance = new Tool()
            {
                Title = "Route",
                Command = new RelayCommand(_ => 
                {
                    var layer = (EditLayer)Map.Layers.FirstOrDefault(l => l.Name == nameof(LayerType.EditLayer));
                        
                    layer.ClearRoute();

                    InfoPanel.CloseRoute();

                    Plotter = new Plotter(InteractiveRoute.Build());

                    Plotter.BeginCreating += (s, e) =>
                    {
                        layer.AddRoute(e.AddInfo);
                        layer.DataHasChanged();
                    };

                    Plotter.Creating += (s, e) =>
                    {
                        var descr = FeatureLengthStepCreating((Feature)e.AddInfo.Feature);

                        void Closing()
                        {
                            layer.ClearRoute();
                            layer.DataHasChanged();
                        }

                        InfoPanel.OpenRoute(descr, Closing);

                        layer.DataHasChanged();
                    };

                    Plotter.EndCreating += (s, e) =>
                    {
                        layer.ClearRouteHelpers();
                        layer.DataHasChanged();

                        ToolManager.ResetAllTools();
                    };

                    Plotter.Hover += (s, e) => layer.DataHasChanged();

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
        public ObservableCollection<Footprint> Footprints { get; set; }

        [Reactive]
        public Footprint SelectedFootprint { get; set; }

        [Reactive]
        public Map Map { get; set; }

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
