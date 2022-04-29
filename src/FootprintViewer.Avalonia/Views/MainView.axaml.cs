using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using System;

namespace FootprintViewer.Avalonia.Views
{
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        private UserMapControl UserMapControl => this.FindControl<UserMapControl>("UserMapControl");
        private TextBlock TextBlockResolution => this.FindControl<TextBlock>("TextBlockResolution");
        private TextBlock TextBlockCoordinates => this.FindControl<TextBlock>("TextBlockCoordinates");
        //private ScaleBarView ScaleBarView => this.FindControl<ScaleBarView>("ScaleBarView");

        public MainView()
        {
            InitializeComponent();

            UserMapControl.PointerMoved += MapControlOnMouseMove;
            UserMapControl.PointerWheelChanged += MapControl_MouseWheel;
            UserMapControl.PropertyChanged += MapControl_PropertyChanged;

            TextBlockResolution.Text = GetCurrentResolution();

            UserMapControl.Viewport.ViewportChanged += Viewport_ViewportChanged;
        }

        private void MainView_ViewportUpdate(object? sender, EventArgs e)
        {
            if (sender is IReadOnlyViewport)
            {
                //        ScaleBarView.Update?.Invoke(viewport);
            }
        }

        private void UserMapControl_ViewportInitialized(object? sender, EventArgs e)
        {
            if (sender is UserMapControl)
            {
                //if (UserMapControl.Map != null)
                {
                    //ScaleBarView.Update(UserMapControl.Map, viewport);
                    //         ScaleBarView.Update(userMapControl.Viewport);
                }
            }
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

            if (e.PropertyName == nameof(UserMapControl.Viewport))
            {

            }
        }

        public event EventHandler<EventArgs>? ViewportUpdate;

        private void Viewport_ViewportChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Viewport viewport)
            {
                //if (UserMapControl.Map != null)
                {
                    //ScaleBarView.Update(UserMapControl.Map, viewport);
                    //  ScaleBarView.Update(viewport);


                    //      ScaleBarView.Update.Invoke(viewport);

                    ViewportUpdate?.Invoke(viewport, EventArgs.Empty);

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

        private void MapControlOnMouseMove(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
            var screenPosition = e.GetPosition(UserMapControl);
            var worldPosition = UserMapControl.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
            var coord = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y).ToMPoint();
            TextBlockCoordinates.Text = FormatHelper.ToCoordinate(coord.X, coord.Y);
            TextBlockResolution.Text = GetCurrentResolution();
        }

        private string GetCurrentResolution()
        {
            var center = UserMapControl.Viewport.Center;
            var point = SphericalMercator.ToLonLat(center.X, center.Y).ToMPoint();
            double groundResolution = UserMapControl.Viewport.Resolution * Math.Cos(point.Y / 180.0 * Math.PI);
            var scale = groundResolution * 96 / 0.0254;
            return FormatHelper.ToScale(scale);
        }
    }
}
