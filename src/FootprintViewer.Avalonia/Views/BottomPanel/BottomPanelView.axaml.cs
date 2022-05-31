using Avalonia.ReactiveUI;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.BottomPanel
{
    public partial class BottomPanelView : ReactiveUserControl<ViewModels.BottomPanel>
    {
        public BottomPanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
