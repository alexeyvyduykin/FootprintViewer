using Avalonia;
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
        private ContentControl MainContentControl => this.FindControl<ContentControl>("MainContentControl");
        private ContentControl PreviewContentControl => this.FindControl<ContentControl>("PreviewContentControl");

        public GroundTargetViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MainContentControl

                this.OneWayBind(ViewModel, vm => vm.MainContent, v => v.MainContentControl.Content).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.MainContentControl.IsVisible).DisposeWith(disposables);

                // PreviewContentControl

                this.OneWayBind(ViewModel, vm => vm.PreviewContent, v => v.PreviewContentControl.Content).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.IsEnable, v => v.PreviewContentControl.IsVisible, value => !value).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
