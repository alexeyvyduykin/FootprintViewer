using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Projection;
using Mapsui.UI;
using System;

namespace FootprintViewer.Avalonia.Views
{
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        private MapControl MapControl => this.FindControl<MapControl>("MapControl");
        private TextBlock TextBlockResolution => this.FindControl<TextBlock>("TextBlockResolution");
        private TextBlock TextBlockCoordinates => this.FindControl<TextBlock>("TextBlockCoordinates");
        private ScaleBarView ScaleBarView => this.FindControl<ScaleBarView>("ScaleBarView");

        public MainView()
        {
            InitializeComponent();

            MapControl.PointerMoved += MapControlOnMouseMove;
            MapControl.PointerWheelChanged += MapControl_MouseWheel;
            MapControl.PropertyChanged += MapControl_PropertyChanged;

            TextBlockResolution.Text = GetCurrentResolution();

            MapControl.Viewport.ViewportChanged += Viewport_ViewportChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void MapControl_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.Bounds))
            {
                TextBlockResolution.Text = GetCurrentResolution();
            }
        }

        private void Viewport_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Viewport viewport)
            {
                if (MapControl.Map != null)
                {
                    ScaleBarView.Update(MapControl.Map, viewport);
                }
            }
        }

        //private void ListBoxFootprints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (sender is System.Windows.Controls.ListBox listBox && listBox.SelectedItem is Footprint footprint)
        //    {
        //        if (footprint.Geometry != null)
        //        {
        //            var point = footprint.Geometry.BoundingBox.Centroid;

        //            MapControl.Navigator.CenterOn(point);
        //        }
        //    }
        //}

        private void MapControl_MouseWheel(object? sender, global::Avalonia.Input.PointerWheelEventArgs e)
        {
            TextBlockResolution.Text = GetCurrentResolution();
        }

        private static void MapControlFeatureInfo(object? sender, FeatureInfoEventArgs e)
        {
            //MessageBox.Show(e.FeatureInfo.ToDisplayText());
        }

        private void MapControlOnMouseMove(object? sender, global::Avalonia.Input.PointerEventArgs e)
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
