using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class FootprintObserverListView : ReactiveUserControl<FootprintObserverList>
    {
        private ProgressBar ProgressBar => this.FindControl<ProgressBar>("ProgressBar");
        private ListBox ListBox => this.FindControl<ListBox>("ListBox");

        public FootprintObserverListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ProgressBar

                this.OneWayBind(ViewModel, vm => vm.IsLoading, v => v.ProgressBar.IsVisible).DisposeWith(disposables);          

                // ListBox

                this.OneWayBind(ViewModel, vm => vm.IsLoading, v => v.ListBox.IsVisible, value => !value).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ListBox.Items).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
