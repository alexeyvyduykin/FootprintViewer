using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class UserGeometryClickInfoPanelView : ReactiveUserControl<UserGeometryClickInfoPanel>
    {
        public UserGeometryClickInfoPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
