using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class FootprintClickInfoPanelView : ReactiveUserControl<FootprintClickInfoPanel>
    {
        private TextBlock SatelliteNodeDirectionTextBlock => this.FindControl<TextBlock>("SatelliteNodeDirectionTextBlock");
        private TextBlock CenterTextBlock => this.FindControl<TextBlock>("CenterTextBlock");
        private TextBlock BeginDurationTextBlock => this.FindControl<TextBlock>("BeginDurationTextBlock");

        public FootprintClickInfoPanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Info.Center,
                    v => v.CenterTextBlock.Text,
                    value => $"{value?.X:0.00}° {value?.Y:0.00}°").DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.Info,
                    v => v.SatelliteNodeDirectionTextBlock.Text,
                    info => $"{info!.SatelliteName} ({info.Node} - {info.Direction})").DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.Info,
                    v => v.BeginDurationTextBlock.Text,
                    info => $"{info!.Begin: dd.MM.yyyy HH:mm:ss} ({info.Duration} sec)").DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
