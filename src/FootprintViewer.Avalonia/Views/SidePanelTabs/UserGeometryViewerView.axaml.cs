using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class UserGeometryViewerView : ReactiveUserControl<UserGeometryViewer>
    {
        public UserGeometryViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ProgressBar
                this.OneWayBind(ViewModel, vm => vm.ViewerList.IsLoading, v => v.ProgressBar.IsVisible).DisposeWith(disposables);

                // ListBox
                this.OneWayBind(ViewModel, vm => vm.ViewerList.IsLoading, v => v.ListBox.IsVisible, value => !value).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ViewerList.Items, v => v.ListBox.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ViewerList.SelectedItem, v => v.ListBox.SelectedItem).DisposeWith(disposables);
            });
        }
    }
}
