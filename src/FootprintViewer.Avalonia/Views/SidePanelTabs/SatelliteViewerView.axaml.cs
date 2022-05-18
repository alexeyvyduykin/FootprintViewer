using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class SatelliteViewerView : ReactiveUserControl<SatelliteViewer>
    {
        public SatelliteViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.ViewerList.Items, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }
    }
}
