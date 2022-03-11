using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views
{
    public partial class SidePanelView : ReactiveUserControl<SidePanel>
    {
        public SidePanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables => { });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
