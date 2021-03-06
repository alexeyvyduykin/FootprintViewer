using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class GroundTargetViewerView : ReactiveUserControl<GroundTargetViewer>
    {
        public GroundTargetViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MainContent
                this.OneWayBind(ViewModel, vm => vm.MainContent, v => v.MainContent.ViewModel).DisposeWith(disposables);
            });
        }
    }
}
