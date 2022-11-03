using Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class CustomToolBarView : UserControl
    {
        public CustomToolBarView()
        {
            InitializeComponent();

            MapBackgroundSelectorButton.Flyout.Closing += Flyout_Closing;

            MapLayerSelectorButton.Flyout.Closing += Flyout_Closing1;
        }

        private void Flyout_Closing1(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            MapLayerSelectorButton.IsChecked = false;
        }

        private void Flyout_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            MapBackgroundSelectorButton.IsChecked = false;
        }
    }
}
