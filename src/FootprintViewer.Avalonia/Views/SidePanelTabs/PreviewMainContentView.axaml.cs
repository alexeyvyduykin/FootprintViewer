using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class PreviewMainContentView : ReactiveUserControl<PreviewMainContent>
    {
        public PreviewMainContentView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
