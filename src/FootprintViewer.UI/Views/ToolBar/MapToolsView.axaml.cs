using Avalonia.Controls;

namespace FootprintViewer.UI.Views.ToolBar
{
    public partial class MapToolsView : UserControl
    {
        public MapToolsView()
        {
            InitializeComponent();

            MenuToggleButton.Flyout.Closed += Flyout_Closed;
        }

        private void Flyout_Closed(object? sender, System.EventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}
