using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using Mapsui;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views
{
    public partial class ScaleMapBarView : ReactiveUserControl<ScaleMapBar>
    {
        public ScaleMapBarView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Position, v => v.TextBlockCoordinates.Text, ConvertToCoordinate).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Resolution, v => v.TextBlockResolution.Text, ConvertToResolution).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Scale, v => v.TextBlockScale.Text).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ScaleLength, v => v.RectangleBeba.Width).DisposeWith(disposables);
            });
        }

        private static string ConvertToCoordinate(MPoint? point)
        {
            if (point == null)
            {
                return string.Empty;
            }

            var (lon, lat) = (point.X, point.Y);

            var lonStr = (lon >= 0.0) ? $"{lon:F5}°E" : $"{Math.Abs(lon):F5}°W";

            var latStr = (lat >= 0.0) ? $"{lat:F5}°N" : $"{Math.Abs(lat):F5}°S";

            return $"{lonStr} {latStr}";
        }

        private static string ConvertToResolution(double scale)
        {
            return $"1:{scale:N0}";
        }
    }
}
