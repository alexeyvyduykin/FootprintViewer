using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.FilterTemplates
{
    public partial class FootprintInfoFilterView : ReactiveUserControl<FootprintTabFilter>
    {
        public FootprintInfoFilterView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
