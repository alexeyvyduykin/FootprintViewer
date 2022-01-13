using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class SceneSearchView : UserControl
    {
        public SceneSearchView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
