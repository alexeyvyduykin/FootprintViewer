using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class CustomToolBarView : ReactiveUserControl<CustomToolBar>
    {
        public CustomToolBarView()
        {
            InitializeComponent();

            this.WhenActivated(disposables => { });
        }
    }
}
