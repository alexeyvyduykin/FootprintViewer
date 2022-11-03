using Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintTabView : UserControl
    {
        public FootprintTabView()
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
