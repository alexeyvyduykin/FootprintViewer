using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class InfoPanelView : ReactiveUserControl<ViewModels.InfoPanel>
    {
        public InfoPanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ItemsControl
                this.OneWayBind(ViewModel, vm => vm.Panels, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }
    }
}
