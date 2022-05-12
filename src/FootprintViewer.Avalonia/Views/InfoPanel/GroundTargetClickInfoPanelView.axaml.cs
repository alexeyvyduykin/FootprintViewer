using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class GroundTargetClickInfoPanelView : ReactiveUserControl<GroundTargetClickInfoPanel>
    {
        public GroundTargetClickInfoPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
