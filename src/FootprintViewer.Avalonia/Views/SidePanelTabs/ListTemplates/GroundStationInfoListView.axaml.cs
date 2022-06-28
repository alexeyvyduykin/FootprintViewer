using Avalonia.ReactiveUI;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ListTemplates
{
    public partial class GroundStationInfoListView : ReactiveUserControl<ViewerList<GroundStation, GroundStationInfo>>
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
