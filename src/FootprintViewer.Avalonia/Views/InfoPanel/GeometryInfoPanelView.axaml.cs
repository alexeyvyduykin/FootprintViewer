using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class GeometryInfoPanelView : ReactiveUserControl<CustomInfoPanel>
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
