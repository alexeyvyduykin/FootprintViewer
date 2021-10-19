﻿using FootprintViewer;
using FootprintViewer.Models;
using FootprintViewer.WPF.ViewModels;
using FootprintViewer.WPF.Controls;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Forms;
using NetTopologySuite.GeometriesGraph;
using NetTopologySuite.Operation.Overlay;
using Mapsui.UI.Wpf;

namespace FootprintViewer.WPF
{
    /*
  + 1) Tools -> ZoomIn/ZoomOut 
    2) Tools -> AOI Rect/Poly/Circle
    3) Hints -> Flyout/Popup
    4) Tools -> RouteDistance
    5) Tools -> LayerList
    6) Hints -> Inner Tutorial
    7) Tools -> ToolInfoList
    8) FootprintList -> Filter
     */


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();

            _mainViewModel.MapInvalidate += Vm_MapInvalidate;

            _mainViewModel.InvalidateMap();

            DataContext = _mainViewModel;

            MapControl.FeatureInfo += MapControlFeatureInfo;
            MapControl.MouseMove += MapControlOnMouseMove;
            MapControl.MouseWheel += MapControl_MouseWheel;
            MapControl.SizeChanged += MapControl_SizeChanged;

            TextBlockResolution.Text = GetCurrentResolution();

            ToolZoomIn.Click += (s, e) => MapControl.Navigator.ZoomIn();
            ToolZoomOut.Click += (s, e) => MapControl.Navigator.ZoomOut();

            ToolRectAOI.Click += (s, e) => MapControl.SetActiveTool(ToolType.DrawingRectangleAOI);
            ToolPolyAOI.Click += (s, e) => MapControl.SetActiveTool(ToolType.DrawingPolygonAOI);
            ToolCircleAOI.Click += (s, e) => MapControl.SetActiveTool(ToolType.DrawingCircleAOI);
            ToolRouteDistance.Click += (s, e) => MapControl.SetActiveTool(ToolType.RoutingDistance);            
            ToolDefault.Click += (s, e) => MapControl.SetActiveTool(ToolType.None);
            ToolEdit.Click += (s, e) => MapControl.SetActiveTool(ToolType.Editing);

            ListBoxFootprints.SelectionChanged += ListBoxFootprints_SelectionChanged;

            MapControl.Viewport.ViewportChanged += Viewport_ViewportChanged;

            InitializeEditSetup();
        }

        private void InitializeEditSetup()
        {          
            var editLayer =  (EditLayer)MapControl.Map.Layers.First(l => l.Name == nameof(LayerType.EditLayer));      
            var observer = new InteractiveFeatureObserver(editLayer);
            observer.CreatingCompleted += FeatureEndCreating;     
            
            MapControl.Observer = observer;
        
            Loaded += (sender, args) =>
            {
                MapControl.Navigator.NavigateTo(observer.Layer.Envelope.Grow(observer.Layer.Envelope.Width * 0.2));
            };
        }

        private void FeatureEndCreating(object? sender, FeatureEventArgs e)
        {
            var feature = e.Feature;
            var bb = feature.Geometry.BoundingBox;
            var coord = ProjectHelper.ToString(bb.Centroid);
            var vertices = feature.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();
            var area = SphericalUtil.ComputeSignedArea(vertices);
            area = Math.Abs(area);
            string str = $"{area:N2} km² | {coord}";
       
            _mainViewModel.AOIDescription = str;
            //_mainViewModel.RouteDescription;
        }

        private void Viewport_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Viewport viewport)
            {
                ScaleBarControl.Update(MapControl.Map, viewport);
            }
        }

        private void ListBoxFootprints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ListBox listBox && listBox.SelectedItem is Footprint footprint)
            {
                if (footprint.Geometry != null)
                {
                    var point = footprint.Geometry.BoundingBox.Centroid;

                    MapControl.Navigator.CenterOn(point);
                }
            }
        }

        private void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlockResolution.Text = GetCurrentResolution();
        }

        private void MapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            TextBlockResolution.Text = GetCurrentResolution();
        }

        private void Vm_MapInvalidate(object sender, EventArgs e)
        {
            if (sender is MainViewModel mainViewModel)
            {
                MapControl.Map = mainViewModel.Map;
            }
        }

        private static void MapControlFeatureInfo(object sender, FeatureInfoEventArgs e)
        {
           //MessageBox.Show(e.FeatureInfo.ToDisplayText());
        }

        private void MapControlOnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var screenPosition = e.GetPosition(MapControl);
            var worldPosition = MapControl.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
            TextBlockCoordinates.Text = ProjectHelper.ToString(worldPosition);
            TextBlockResolution.Text = GetCurrentResolution();
        }

        private string GetCurrentResolution()
        {            
            var center = MapControl.Viewport.Center;
            var point = SphericalMercator.ToLonLat(center.X, center.Y);      
            //var mapInfo = MapControl.GetMapInfo(center);
            //var res = mapInfo.Resolution;    
            //var zoomlevel = ToZoomLevel(res);
            double groundResolution = MapControl.Viewport.Resolution * Math.Cos(point.Y / 180.0 * Math.PI);
            var scale = groundResolution * 96 / 0.0254;
            //var scale = MapScale(point.Y, zoomlevel, 96, 256);         
            return $"1:{scale:N0}";
        }
    }
}
