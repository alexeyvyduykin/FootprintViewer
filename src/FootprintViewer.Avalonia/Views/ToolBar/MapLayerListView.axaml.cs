using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class MapLayerListView : ReactiveUserControl<MapLayerList>
    {
        public MapLayerListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ItemsControl
                this.OneWayBind(ViewModel, vm => vm.Layers, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }
    }
}
