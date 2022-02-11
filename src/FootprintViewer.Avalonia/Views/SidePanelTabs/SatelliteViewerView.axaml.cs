using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class SatelliteViewerView : ReactiveUserControl<SatelliteViewer>
    {
        private ItemsControl SatelliteItemsControl => this.FindControl<ItemsControl>("SatelliteItemsControl");

        public SatelliteViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // SatelliteItemsControl
                this.OneWayBind(ViewModel, vm => vm.SatelliteInfos, v => v.SatelliteItemsControl.Items).DisposeWith(disposables);             
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
