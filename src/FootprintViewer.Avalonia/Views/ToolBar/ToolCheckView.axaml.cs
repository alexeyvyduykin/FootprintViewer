using Avalonia.ReactiveUI;
using FootprintViewer.Models;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class ToolCheckView : ReactiveUserControl<IToolCheck>
    {
        public ToolCheckView()
        {
            InitializeComponent();

            this.WhenActivated(disposables => { });
        }
    }
}
