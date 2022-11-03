using Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private static void UpdateMainWindow()
        {
            if (Design.IsDesignMode == true)
            {
                return;
            }

            App.GetMainWindow().UpdateComponent();
        }
    }
}
