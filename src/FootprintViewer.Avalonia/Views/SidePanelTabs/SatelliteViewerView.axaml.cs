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
        private ItemsControl ItemsControl => this.FindControl<ItemsControl>("ItemsControl");

        public SatelliteViewerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.SatelliteInfos, v => v.ItemsControl.Items).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
