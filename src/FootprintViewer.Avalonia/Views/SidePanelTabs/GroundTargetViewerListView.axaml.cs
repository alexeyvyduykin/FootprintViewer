using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class GroundTargetViewerListView : ReactiveUserControl<GroundTargetViewerList>
    {
        private ListBox GroundTargetListBox => this.FindControl<ListBox>("GroundTargetListBox");

        public GroundTargetViewerListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // GroundTargetListBox
                this.OneWayBind(ViewModel, vm => vm.GroundTargetInfos, v => v.GroundTargetListBox.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedGroundTargetInfo, v => v.GroundTargetListBox.SelectedItem).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
