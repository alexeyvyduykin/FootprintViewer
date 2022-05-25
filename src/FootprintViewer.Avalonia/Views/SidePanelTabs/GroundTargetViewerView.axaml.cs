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
                //this.OneWayBind(ViewModel, vm => vm.Preview, v => v.PreviewContent.ViewModel).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Preview, v => v.PreviewContent.DataContext).DisposeWith(disposables);

                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.MainContentControl.IsVisible).DisposeWith(disposables);
                //this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.MainContentControl.ViewModel).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.MainContentControl.DataContext).DisposeWith(disposables);
            });
        }
    }
}
