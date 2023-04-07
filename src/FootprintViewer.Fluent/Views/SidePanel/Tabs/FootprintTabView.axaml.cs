using Avalonia.Controls;

namespace FootprintViewer.Fluent.Views.SidePanel.Tabs
{
    public partial class FootprintTabView : UserControl
    {
        public FootprintTabView()
        {
            InitializeComponent();

            SearchToggleButton.Flyout.Closed += Flyout_Closed;
        }

        private void Flyout_Closed(object? sender, System.EventArgs e)
        {
            SearchToggleButton.IsChecked = false;
        }
    }
}
