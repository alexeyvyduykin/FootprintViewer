using Avalonia.Controls;

namespace FootprintViewer.UI.Views.SidePanel.Tabs
{
    public partial class FootprintPreviewTabView : UserControl
    {
        public FootprintPreviewTabView()
        {
            InitializeComponent();

            SearchToggleButton.Flyout.Closing += Flyout_Closing;
        }

        private void Flyout_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SearchToggleButton.IsChecked = false;
        }
    }
}
