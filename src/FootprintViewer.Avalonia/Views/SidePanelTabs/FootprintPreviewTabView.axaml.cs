using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintPreviewTabView : ReactiveUserControl<FootprintPreviewTab>
    {
        public FootprintPreviewTabView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.MainContentControl.ViewModel).DisposeWith(disposables);

                // ToggleButton
                SearchToggleButton.Flyout?.Events().Closing.Subscribe(_ => SearchToggleButton.IsChecked = false).DisposeWith(disposables);
            });
        }
    }
}
