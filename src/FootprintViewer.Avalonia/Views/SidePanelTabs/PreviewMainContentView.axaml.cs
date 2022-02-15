using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class PreviewMainContentView : ReactiveUserControl<PreviewMainContent>
    {
        public PreviewMainContentView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
