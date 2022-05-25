using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ListTemplates
{
    public partial class FootprintPreviewListView : ReactiveUserControl<ViewerList<FootprintPreview>>
    {
        public FootprintPreviewListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // ListBox
                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ListBox.Items).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedItem, v => v.ListBox.SelectedItem).DisposeWith(disposables);
            });
        }
    }
}
