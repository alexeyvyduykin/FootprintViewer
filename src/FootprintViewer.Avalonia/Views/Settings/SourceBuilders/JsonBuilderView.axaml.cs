using Avalonia.Controls;
using FootprintViewer.AppStates;
using FootprintViewer.Avalonia.Helpers;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia.Views.Settings.SourceBuilders
{
    public partial class JsonBuilderView : UserControl
    {
        public JsonBuilderView()
        {
            InitializeComponent();

            var mainState = Locator.Current.GetService<MainState>();

            if (mainState?.LastOpenDirectory != null)
            {
                DirectoryTextBox.Text = mainState.LastOpenDirectory;
            }

            SearchButton.Command = ReactiveCommand.CreateFromTask(GetFolderNameAsync);
        }

        private async Task GetFolderNameAsync()
        {
            var mainState = Locator.Current.GetService<MainState>();

            var res = await FileDialogHelper.ShowOpenFolderDialogAsync("Create folder with *.json files", mainState?.LastOpenDirectory);

            if (string.IsNullOrEmpty(res) == false)
            {
                DirectoryTextBox.Text = res;

                if (mainState != null)
                {
                    mainState.LastOpenDirectory = res;
                }
            }
        }
    }
}
