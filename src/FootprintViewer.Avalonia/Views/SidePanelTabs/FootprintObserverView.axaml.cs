using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
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
        private ToggleButton SearchToggleButton => this.FindControl<ToggleButton>("SearchToggleButton");

        private ViewModelViewHost MainContentControl => this.FindControl<ViewModelViewHost>("MainContentControl");

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
