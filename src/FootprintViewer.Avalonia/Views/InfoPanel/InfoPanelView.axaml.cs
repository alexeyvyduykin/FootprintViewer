using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class InfoPanelView : UserControl
    {
        public InfoPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
