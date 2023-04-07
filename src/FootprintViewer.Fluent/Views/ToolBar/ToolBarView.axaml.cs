using Avalonia.Controls;

namespace FootprintViewer.Fluent.Views.ToolBar
{
    public partial class ToolBarView : UserControl
    {
        public ToolBarView()
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
