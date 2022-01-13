using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FootprintViewer.Avalonia.Views
{
    public partial class ToolBarView : UserControl
    {
        public ToolBarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
