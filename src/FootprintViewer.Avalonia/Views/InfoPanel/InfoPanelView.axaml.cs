using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.InfoPanel
{
    public partial class InfoPanelView : ReactiveUserControl<FootprintViewer.ViewModels.InfoPanel>
    {
        private ItemsControl ItemsControl => this.FindControl<ItemsControl>("ItemsControl");

        public InfoPanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ItemsControl
                this.OneWayBind(ViewModel, vm => vm.Panels, v => v.ItemsControl.Items).DisposeWith(disposables);              
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
