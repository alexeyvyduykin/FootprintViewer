using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class FootprintItemView : ReactiveUserControl<FootprintViewModel>
    {
        public FootprintItemView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Center, v => v.CenterTextBlock.Text, value => $"{value?.X:0.00}° {value?.Y:0.00}°").DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Begin, v => v.BeginTextBlock.Text, value => value.ToString("dd.MM.yyyy HH:mm:ss")).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Duration, v => v.DurationTextBlock.Text, value => $"{value} sec").DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Direction, v => v.DirectionTextBlock.Text, value => value.ToString()).DisposeWith(disposables);
            });
        }
    }
}
