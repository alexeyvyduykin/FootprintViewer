using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ListTemplates
{
    public partial class GroundStationInfoListView : ReactiveUserControl<GroundStationViewerList>
    {
        public GroundStationInfoListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ItemsControl
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }
    }
}
