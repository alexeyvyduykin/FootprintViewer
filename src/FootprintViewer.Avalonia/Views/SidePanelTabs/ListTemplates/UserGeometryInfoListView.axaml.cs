using Avalonia.ReactiveUI;
using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ListTemplates
{
    public partial class UserGeometryInfoListView : ReactiveUserControl<ViewerList<UserGeometry, UserGeometryInfo>>
    {
        public UserGeometryInfoListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ProgressBar
                this.OneWayBind(ViewModel, vm => vm.IsLoading, v => v.ProgressBar.IsVisible).DisposeWith(disposables);

                // ListBox
                this.OneWayBind(ViewModel, vm => vm.IsLoading, v => v.ListBox.IsVisible, value => !value).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ListBox.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedItem, v => v.ListBox.SelectedItem).DisposeWith(disposables);
            });
        }
    }
}
