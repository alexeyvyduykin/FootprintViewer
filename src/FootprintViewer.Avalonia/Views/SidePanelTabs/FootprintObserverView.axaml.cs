using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintObserverView : ReactiveUserControl<FootprintObserver>
    {
        public FootprintObserverView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ToggleButton
                SearchToggleButton.Events().Click.Select(args => Unit.Default).InvokeCommand(ViewModel, vm => vm.FilterClick).DisposeWith(disposables);

                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.MainContent, v => v.MainContentControl.ViewModel).DisposeWith(disposables);
            });

        }
    }
}
