using Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class CustomToolBar2View : UserControl
    {
        public CustomToolBar2View()
        {
            InitializeComponent();

            MapBackgroundSelectorButton.Flyout.Closed += Flyout_Closed;

            MapLayerSelectorButton.Flyout.Closed += Flyout_Closed1;
        }

        private void Flyout_Closed(object? sender, System.EventArgs e)
        {
            MapBackgroundSelectorButton.IsChecked = false;
        }

        private void Flyout_Closed1(object? sender, System.EventArgs e)
        {
            MapLayerSelectorButton.IsChecked = false;
        }
    }
}
