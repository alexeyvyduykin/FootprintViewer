using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mapsui.Projection;
using Mapsui.UI;
using Mapsui;
using System;

namespace FootprintViewer.Avalonia.Views
{
    public partial class MainWindow : Window
    {
  //      private readonly MapControl _mapControl;
        private readonly TextBlock _textBlockResolution;
        private readonly TextBlock _textBlockCoordinates;
        private readonly ScaleBarView _scaleBarView;

        public MainWindow()
        {
            WindowsManager.AllWindows.Add(this);

            InitializeComponent();

  //          _mapControl = this.FindControl<MapControl>("MapControl");
            _textBlockResolution = this.FindControl<TextBlock>("TextBlockResolution");
            _textBlockCoordinates = this.FindControl<TextBlock>("TextBlockCoordinates");
            _scaleBarView = this.FindControl<ScaleBarView>("ScaleBarView");

            //_mapControl.FeatureInfo += MapControlFeatureInfo;

  //          _mapControl.PointerMoved += MapControlOnMouseMove;
  //          _mapControl.PointerWheelChanged += MapControl_MouseWheel;
  //          _mapControl.PropertyChanged += MapControl_PropertyChanged;

            _textBlockResolution.Text = GetCurrentResolution();
                
  //          _mapControl.Viewport.ViewportChanged += Viewport_ViewportChanged;
            
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void MapControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindow.Bounds))
            {
                _textBlockResolution.Text = GetCurrentResolution();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Viewport_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Viewport viewport)
            {
   //             if (_mapControl.Map != null)
                {
    //                _scaleBarView.Update(_mapControl.Map, viewport);
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
            _textBlockResolution.Text = GetCurrentResolution();
        }

        private static void MapControlFeatureInfo(object? sender, FeatureInfoEventArgs e)
        {
            //MessageBox.Show(e.FeatureInfo.ToDisplayText());
        }

        private void MapControlOnMouseMove(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
  //          var screenPosition = e.GetPosition(_mapControl);
  //          var worldPosition = _mapControl.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
  //          var coord = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
  //          _textBlockCoordinates.Text = FormatHelper.ToCoordinate(coord.X, coord.Y);
            _textBlockResolution.Text = GetCurrentResolution();
        }

        private string GetCurrentResolution()
        {
            //        var center = _mapControl.Viewport.Center;
            //        var point = SphericalMercator.ToLonLat(center.X, center.Y);
            //var mapInfo = MapControl.GetMapInfo(center);
            //var res = mapInfo.Resolution;    
            //var zoomlevel = ToZoomLevel(res);
            //         double groundResolution = _mapControl.Viewport.Resolution * Math.Cos(point.Y / 180.0 * Math.PI);
            //         var scale = groundResolution * 96 / 0.0254;
            //var scale = MapScale(point.Y, zoomlevel, 96, 256);         
            //          return FormatHelper.ToScale(scale);

            return "";
        }
    }
}
