using Avalonia;
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
    public partial class SceneSearchView : ReactiveUserControl<SceneSearch>
    {
        public SceneSearchView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ToggleButtonSearch
                this.ToggleButtonSearch.Events().Click.Select(args => Unit.Default).InvokeCommand(ViewModel, vm => vm.FilterClick).DisposeWith(disposables);

                // MainContentControl
                this.OneWayBind(ViewModel, vm => vm.ViewerList, v => v.MainContentControl.ViewModel).DisposeWith(disposables);
            });
        }
    }
}
