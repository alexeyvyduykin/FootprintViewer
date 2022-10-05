using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;
using System.IO;

namespace DataSettingsSample.Views.SourceBuilders
{
    public partial class JsonBuilderView : ReactiveUserControl<JsonBuilderViewModel>
    {
        public JsonBuilderView()
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
                var path = await App.OpenFolderDialog(null);

                if (Directory.Exists(path) == false)
                {
                    return;
                }

                ViewModel.Directory = path;
            }
        }
    }
}
