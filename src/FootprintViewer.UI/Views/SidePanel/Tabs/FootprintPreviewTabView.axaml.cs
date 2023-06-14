using Avalonia.Controls;

namespace FootprintViewer.UI.Views.SidePanel.Tabs
{
    public partial class FootprintPreviewTabView : UserControl
    {
        public FootprintPreviewTabView()
        {
            InitializeComponent();

            if (SearchToggleButton.Flyout is { })
            {
                SearchToggleButton.Flyout.Closed += Flyout_Closed;
            }
        }

        private void Flyout_Closed(object? sender, EventArgs e)
        {
            SearchToggleButton.IsChecked = false;
        }
    }
}
