using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class FootprintClickInfoPanelView : ReactiveUserControl<FootprintClickInfoPanel>
    {
        public FootprintClickInfoPanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.FootprintViewModel.Center,
                    v => v.CenterTextBlock.Text,
                    value => $"{value?.X:0.00}° {value?.Y:0.00}°").DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.FootprintViewModel,
                    v => v.SatelliteNodeDirectionTextBlock.Text,
                    info => $"{info!.SatelliteName} ({info.Node} - {info.Direction})").DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.FootprintViewModel,
                    v => v.BeginDurationTextBlock.Text,
                    info => $"{info!.Begin: dd.MM.yyyy HH:mm:ss} ({info.Duration} sec)").DisposeWith(disposables);
            });
        }
    }
}
