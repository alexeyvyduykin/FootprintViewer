using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ListTemplates
{
    public partial class GroundStationInfoListView : ReactiveUserControl<ViewerList<GroundStationInfo>>
    {
        public GroundStationInfoListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }
    }
}
