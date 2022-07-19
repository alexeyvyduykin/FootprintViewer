using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.IO;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class FolderSourceView : ReactiveUserControl<FolderSourceViewModel>
    {
        public FolderSourceView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }

        public async void AddPath_Clicked(object sender, RoutedEventArgs args)
        {
            if (ViewModel != null)
            {
                string _path = await App.OpenFolderDialog(null);

                if (Directory.Exists(_path) == false)
                {
                    return;
                }

                ViewModel.Directory = _path;
            }
        }
    }
}
