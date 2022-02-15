using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views
{
    public partial class SidePanelView : ReactiveUserControl<SidePanel>
    {
        private TabControl SidePanel => this.FindControl<TabControl>("SidePanel");

        public SidePanelView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // SidePanel
                this.OneWayBind(ViewModel, vm => vm.Tabs, v => v.SidePanel.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedTab, v => v.SidePanel.SelectedItem).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
