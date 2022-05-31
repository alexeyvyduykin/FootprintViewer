using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.BottomPanel
{
    public partial class SnapshotMakerView : ReactiveUserControl<SnapshotMaker>
    {
        public SnapshotMakerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
