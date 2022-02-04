﻿using FootprintViewer.Interactivity;
using FootprintViewer.Interactivity.Decorators;
using FootprintViewer.Interactivity.Designers;
using FootprintViewer.InteractivityEx;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
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
        private readonly CustomToolBar _customToolBar;
        private readonly MapListener? _mapListener;
        private readonly FootprintObserver? _footprintObserver;
        private readonly SceneSearch? _sceneSearch;
        private readonly CustomProvider _provider;
        private IFeature? _currentFeature;

        public MainViewModel(IReadonlyDependencyResolver dependencyResolver)
        {
            _factory = dependencyResolver.GetExistingService<ProjectFactory>();
            _map = dependencyResolver.GetExistingService<Map>();
            _sidePanel = dependencyResolver.GetExistingService<SidePanel>();
            _customToolBar = dependencyResolver.GetExistingService<CustomToolBar>();
            _provider = dependencyResolver.GetExistingService<CustomProvider>();
            _footprintObserver = dependencyResolver.GetExistingService<FootprintObserver>();
            _sceneSearch = dependencyResolver.GetExistingService<SceneSearch>();
      
            _infoPanel = _factory.CreateInfoPanel();

            _customToolBar.ZoomInClick.Subscribe(_ => ZoomInCommand());
            _customToolBar.ZoomOutClick.Subscribe(_ => ZoomOutCommand());
            _customToolBar.AddRectangleCheck.Subscribe(_ => RectangleCommand());
            _customToolBar.AddPolygonCheck.Subscribe(_ => PolygonCommand());
            _customToolBar.AddCircleCheck.Subscribe(_ => CircleCommand());
            _customToolBar.RouteDistanceCheck.Subscribe(_ => RouteDistanceCommand());
            _customToolBar.LayerChanged.Subscribe(layer => _map.SetWorldMapLayer(layer));

            _editLayer = _map.GetLayer<EditLayer>(LayerType.Edit);

            _map.DataChanged += Map_DataChanged;

            _mapListener = new MapListener();

            _mapListener.LeftClickOnMap += MapListener_LeftClickOnMap;

            AOIChanged += (s, e) =>
            {
                if (s != null)
                {
                    if (s is IGeometry geometry)
                    {
                        _sceneSearch?.SetAOI(geometry);
                    }
                }
                else
                {
                    _sceneSearch?.ResetAOI();
                }
            };

            ActualController = new EditController2();

            _customToolBar.SelectGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    ActualController = new EditController2();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.TranslateGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    ActualController = new EditController2();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.RotateGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    ActualController = new EditController2();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.ScaleGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    ActualController = new EditController2();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.EditGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    ActualController = new EditController2();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.RectangleGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    DrawingRectangleCommand();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.CircleGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    DrawingCircleCommand();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.PolygonGeometryCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    DrawingPolygonCommand();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });

            _customToolBar.RouteDistanceCheck.Subscribe(tool =>
            {
                if (tool.IsCheck == true)
                {
                    DrawingRouteCommand();
                }
                else
                {
                    _currentFeature = null;
                    InteractiveLayerRemove();
                }
            });
        }

        private void MapListener_LeftClickOnMap(object? sender, EventArgs e)
        {
            if (sender is MapInfo mapInfo)
            {
                var feature = mapInfo.Feature;

                IDecorator? decorator = null;

                if (_customToolBar.SelectGeometry.IsCheck == true)
                {
                    InteractiveLayerRemove();

                    if (feature != _currentFeature)
                    {
                        Map.Layers.Add(CreateSelectLayer(mapInfo.Layer, mapInfo.Feature));

                        _currentFeature = feature;
                    }
                    else
                    {
                        _currentFeature = null;
                    }

                    return;
                }
                else if (_customToolBar.ScaleGeometry.IsCheck == true)
                {
                    decorator = new ScaleDecorator(feature);
                }
                else if (_customToolBar.TranslateGeometry.IsCheck == true)
                {
                    decorator = new TranslateDecorator(feature);
                }
                else if (_customToolBar.RotateGeometry.IsCheck == true)
                {
                    decorator = new RotateDecorator(feature);
                }
                else if (_customToolBar.EditGeometry.IsCheck == true)
                {
                    decorator = new EditDecorator(feature);
                }

                if (decorator == null)
                {
                    return;
                }

                InteractiveLayerRemove();

                if (feature != _currentFeature)
                {
                    Map.Layers.Add(new InteractiveLayer(mapInfo.Layer, decorator) { Name = "InteractiveLayer" });

                    MapObserver = new MapObserver(decorator);

                    _currentFeature = feature;
                }
                else
                {
                    _currentFeature = null;
                }
            }
            else if (sender is string name)
            {
                if (_footprintObserver != null && _footprintObserver.IsActive == true)
                {
                    if (Plotter != null && (Plotter.IsCreating == true || Plotter.IsEditing == true))
                    {
                        return;
                    }

                    _footprintObserver.SelectFootprintInfo(name);
                }
            }
        }

        private static ILayer CreateSelectLayer(ILayer source, IFeature feature)
        {
            return new SelectLayer(source, feature)
            {
                Name = "InteractiveLayer",
                IsMapInfoLayer = true,
                Style = new VectorStyle()
                {
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Transparent),
                    Outline = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Green, 4),
                    Line = new Mapsui.Styles.Pen(Mapsui.Styles.Color.Green, 4),
                },
            };
        }

        private void InteractiveLayerRemove()
        {
            var layer = Map.Layers.FindLayer("InteractiveLayer").FirstOrDefault();

            if (layer != null)
            {
                Map.Layers.Remove(layer);
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
            var geometry = (Mapsui.Geometries.LineString)addInfo.Feature.Geometry;
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

                ToolBar.Uncheck();

                ActualController = new EditController();
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

                ToolBar.Uncheck();

                ActualController = new EditController();
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

                Tip = null;

                ToolBar.Uncheck();

                ActualController = new EditController();
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

                Tip = null;

                ToolBar.Uncheck();

                ActualController = new EditController();
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

                ToolBar.Uncheck();

                ActualController = new EditController();
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

                ToolBar.Uncheck();

                ActualController = new EditController();
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

        private void DrawingRectangleCommand()
        {
            InteractiveLayerRemove();

            var designer = new RectangleDesigner();

            var layer = Map.Layers.FindLayer("FeatureLayer").FirstOrDefault();

            Map.Layers.Add(new InteractiveLayer(layer, designer) { Name = "InteractiveLayer" });

            designer.HoverCreating += (s, e) =>
            {
                var feature = designer.Feature;

                var area = GetFeatureArea2(feature);

                Tip = new Tip() { Text = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²" };
            };

            designer.EndCreating += (s, e) =>
            {
                _provider.AddFeature(designer.Feature.Copy());

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать прямоугольник" };

            MapObserver = new MapObserver(designer);

            ActualController = new DrawingController2();
        }

        private void DrawingCircleCommand()
        {
            InteractiveLayerRemove();

            var designer = new CircleDesigner();

            var layer = Map.Layers.FindLayer("FeatureLayer").FirstOrDefault();

            Map.Layers.Add(new InteractiveLayer(layer, designer) { Name = "InteractiveLayer" });

            designer.HoverCreating += (s, e) =>
            {
                var feature = designer.Feature;

                var area = GetFeatureArea2(feature);

                Tip = new Tip() { Text = $"Отпустите клавишу мыши для завершения рисования. Область: {area:N2} km²" };
            };

            designer.EndCreating += (s, e) =>
            {
                _provider.AddFeature(designer.Feature.Copy());

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать круг" };

            MapObserver = new MapObserver(designer);

            ActualController = new DrawingController2();
        }

        private void DrawingRouteCommand()
        {
            InteractiveLayerRemove();

            var designer = new RouteDesigner();

            var layer = Map.Layers.FindLayer("FeatureLayer").FirstOrDefault();

            Map.Layers.Add(new InteractiveLayer(layer, designer) { Name = "InteractiveLayer" });

            designer.HoverCreating += (s, e) =>
            {
                var distance = GetRouteLength2(designer);

                var res = (distance >= 1) ? $"{distance:N2} km" : $"{distance * 1000.0:N2} m";

                Tip = new Tip() { Text = $"Расстояние: {res}" };
            };

            designer.EndCreating += (s, e) =>
            {
                _provider.AddFeature(designer.Feature.Copy());

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Кликните, чтобы начать измерение" };

            MapObserver = new MapObserver(designer);

            ActualController = new DrawingController2();
        }

        private void DrawingPolygonCommand()
        {
            InteractiveLayerRemove();

            var designer = new PolygonDesigner();

            var layer = Map.Layers.FindLayer("FeatureLayer").FirstOrDefault();

            Map.Layers.Add(new InteractiveLayer(layer, designer) { Name = "InteractiveLayer" });

            designer.BeginCreating += (s, e) =>
            {
                Tip = new Tip() { Text = "Нажмите, чтобы продолжить рисовать фигуру" };
            };

            designer.Creating += (s, e) =>
            {
                if (designer.Feature.Geometry.AllVertices().Count() > 2)
                {
                    var area = GetFeatureArea2(designer.Feature);

                    Tip = new Tip() { Text = $"Щелкните по первой точке, чтобы закрыть эту фигуру. Область: {area:N2} km²" };
                }
            };

            designer.EndCreating += (s, e) =>
            {
                _provider.AddFeature(designer.Feature.Copy());

                Tip = null;

                _customToolBar.Uncheck();
            };

            Tip = new Tip() { Text = "Нажмите и перетащите, чтобы нарисовать полигон" };

            MapObserver = new MapObserver(designer);

            ActualController = new DrawingController2();
        }

        private static double GetFeatureArea2(IFeature feature)
        {
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            return Math.Abs(area);
        }

        private static double GetRouteLength2(RouteDesigner designer)
        {
            var geometry = (Mapsui.Geometries.LineString)designer.Feature.Geometry;
            var fHelp = designer.ExtraFeatures.Single();
            var verts0 = geometry.AllVertices();
            var verts1 = fHelp.Geometry.AllVertices();
            var verts = verts0.Union(verts1);
            var vertices = verts.Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            return SphericalUtil.ComputeDistance(vertices);
        }

        public Map Map => _map;

        public SidePanel SidePanel => _sidePanel;

        public InfoPanel InfoPanel => _infoPanel;

        public MapListener? MapListener => _mapListener;

        public CustomToolBar ToolBar => _customToolBar;

        [Reactive]
        public ObservableCollection<MapLayer>? MapLayers { get; set; }

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public Plotter? Plotter { get; set; }

        [Reactive]
        public Tip? Tip { get; set; }

        [Reactive]
        public IMapObserver? MapObserver { get; set; }
    }

    public class MapLayer
    {
        public string? Name { get; set; }

        public string? CRS { get; set; }

        public string? Format { get; set; }
    }
}
