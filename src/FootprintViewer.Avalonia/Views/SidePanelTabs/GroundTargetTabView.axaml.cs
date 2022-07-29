using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs
{
    public partial class GroundTargetTabView : ReactiveUserControl<GroundTargetTab>
    {
        public GroundTargetTabView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
