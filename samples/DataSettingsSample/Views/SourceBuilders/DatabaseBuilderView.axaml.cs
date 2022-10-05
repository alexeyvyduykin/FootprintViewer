using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;

namespace DataSettingsSample.Views.SourceBuilders
{
    public partial class DatabaseBuilderView : ReactiveUserControl<DatabaseBuilderViewModel>
    {
        public DatabaseBuilderView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
