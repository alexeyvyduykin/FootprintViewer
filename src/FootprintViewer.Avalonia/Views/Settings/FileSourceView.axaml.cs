using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.IO;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class FileSourceView : ReactiveUserControl<FileSourceViewModel>
    {
        public FileSourceView()
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
                string _path = await App.OpenFileDialog(null, ViewModel.FilterName, ViewModel.FilterExtension);

                if (File.Exists(_path) == false)
                {
                    return;
                }

                ViewModel.Path = _path;
            }
        }
    }
}
