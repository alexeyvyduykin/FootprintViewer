using Avalonia.Media;
using Avalonia.ReactiveUI;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class SatelliteItemView : ReactiveUserControl<SatelliteViewModel>
    {
        public SatelliteItemView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.RectangleBorder.Background, ConvertToBrush).DisposeWith(disposables);
            });
        }

        private static IBrush ConvertToBrush(string? name)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                return new SolidColorBrush() { Color = Colors.Black };
            }

            var color = LayerStyleManager.SatellitePalette.PickColor(name);

            return new SolidColorBrush()
            {
                Color = Color.FromRgb(color.R, color.G, color.B)
            };
        }
    }
}
