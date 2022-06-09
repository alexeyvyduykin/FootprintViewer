using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class FolderSourceInfoView : ReactiveUserControl<FolderSourceInfo>
    {
        public FolderSourceInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
