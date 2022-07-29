using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class GroundTargetItemView : ReactiveUserControl<GroundTargetViewModel>
    {
        public GroundTargetItemView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
