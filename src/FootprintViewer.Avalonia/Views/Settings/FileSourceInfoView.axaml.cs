using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels.Settings;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class FileSourceInfoView : ReactiveUserControl<FileSourceInfo>
    {
        public FileSourceInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
