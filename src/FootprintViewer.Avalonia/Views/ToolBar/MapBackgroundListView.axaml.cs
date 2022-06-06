using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class MapBackgroundListView : ReactiveUserControl<MapBackgroundList>
    {
        public MapBackgroundListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables => { });
        }
    }
}
