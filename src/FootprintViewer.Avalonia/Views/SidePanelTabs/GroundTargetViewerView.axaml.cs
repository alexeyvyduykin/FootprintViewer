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
                // PreviewContent
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.PreviewContent.IsVisible, value => !value).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Preview, v => v.PreviewContent.DataContext).DisposeWith(disposables);

                // ViewerList
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.ViewerList.IsVisible).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.ViewerList.DataContext).DisposeWith(disposables);
            });
        }
    }
}
