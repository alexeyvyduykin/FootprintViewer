using Avalonia.Media;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class GroundStationItemView : ReactiveUserControl<GroundStationViewModel>
    {
        public GroundStationItemView()
        {
            InitializeComponent();

            Slider1.Minimum = 0;
            Slider1.Maximum = 50;

            Slider2.Minimum = 0;
            Slider2.Maximum = 50;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Center, v => v.CoordinateTextBlock.Text, ConvertToCoordinateInfo).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.AreaItems, v => v.ItemsControl.Items, ConvertToAvaloniaItems).DisposeWith(disposables);
            });
        }

        private static string ConvertToCoordinateInfo(NetTopologySuite.Geometries.Coordinate? coordinate)
        {
            if (coordinate == null)
            {
                return string.Empty;
            }

            return $"Lon:{coordinate.X,7: 0.00;-0.00; 0.00}° Lat:{coordinate.Y,6: 0.00;-0.00; 0.00}°";
        }

        private static IList<AvaloniaAreaItem> ConvertToAvaloniaItems(List<GroundStationAreaItem>? items)
        {
            if (items != null)
            {
                return items.Select(s => new AvaloniaAreaItem()
                {
                    Brush = new SolidColorBrush()
                    {
                        Color = Color.FromRgb((byte)s.Color.R, (byte)s.Color.G, (byte)s.Color.B)
                    },
                    Angle = s.Angle,
                }).ToList();
            }

            return new List<AvaloniaAreaItem>();
        }
    }

    public class AvaloniaAreaItem
    {
        public IBrush? Brush { get; set; }

        public double Angle { get; set; }
    }
}
