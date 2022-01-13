using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class GeometryInfoPanelView : UserControl
    {
        public GeometryInfoPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
