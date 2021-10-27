using FootprintViewer;
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
using Mapsui.Geometries;
using NetTopologySuite.Algorithm;

namespace FootprintViewer.WPF
{
    /*   
    5) Tools -> LayerList
    6) Hints -> Inner Tutorial
    8) FootprintList -> Filter
    */


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();

            MapControl.FeatureInfo += MapControlFeatureInfo;
            MapControl.MouseMove += MapControlOnMouseMove;
            MapControl.MouseWheel += MapControl_MouseWheel;
            MapControl.SizeChanged += MapControl_SizeChanged;

            TextBlockResolution.Text = GetCurrentResolution();

            ListBoxFootprints.SelectionChanged += ListBoxFootprints_SelectionChanged;

            MapControl.Viewport.ViewportChanged += Viewport_ViewportChanged;
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

        private static void MapControlFeatureInfo(object sender, FeatureInfoEventArgs e)
        {
           //MessageBox.Show(e.FeatureInfo.ToDisplayText());
        }

        private void MapControlOnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var screenPosition = e.GetPosition(MapControl);
            var worldPosition = MapControl.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
            var coord = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
            TextBlockCoordinates.Text = FormatHelper.ToCoordinate(coord.X, coord.Y);
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
            return FormatHelper.ToScale(scale);
        }
    }
}
