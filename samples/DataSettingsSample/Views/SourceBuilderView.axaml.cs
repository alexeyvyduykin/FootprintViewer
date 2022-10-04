using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;

namespace DataSettingsSample.Views
{
    public partial class SourceBuilderView : ReactiveUserControl<SourceBuilderViewModel>
    {
        public SourceBuilderView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
