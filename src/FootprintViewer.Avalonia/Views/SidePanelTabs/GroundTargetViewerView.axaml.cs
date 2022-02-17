using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class GroundTargetViewerView : ReactiveUserControl<GroundTargetViewer>
    {
        private ViewModelViewHost MainContentControl => this.FindControl<ViewModelViewHost>("MainContentControl");

        public GroundTargetViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MainContentControl

                this.OneWayBind(ViewModel, vm => vm.MainContent, v => v.MainContentControl.ViewModel).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
