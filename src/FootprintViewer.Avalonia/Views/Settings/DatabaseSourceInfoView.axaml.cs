using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class DatabaseSourceInfoView : ReactiveUserControl<DatabaseSourceInfo>
    {
        public DatabaseSourceInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
