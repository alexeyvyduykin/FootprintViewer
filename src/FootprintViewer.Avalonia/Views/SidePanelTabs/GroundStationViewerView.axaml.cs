using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class GroundStationViewerView : ReactiveUserControl<GroundStationViewer>
    {
        public GroundStationViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.MainContentControl.ViewModel).DisposeWith(disposables);
            });
        }
    }
}
