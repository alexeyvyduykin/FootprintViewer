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
        private readonly string? _lastOpenDirectory;

        public JsonBuilderView()
        {
            InitializeComponent();

            var mainState = Locator.Current.GetService<MainState>();

            _lastOpenDirectory = mainState?.LastOpenDirectory;

            SearchButton.Command = ReactiveCommand.CreateFromTask(GetFolderNameAsync);
        }

        private async Task GetFolderNameAsync()
        {
            var res = await FileDialogHelper.ShowOpenFolderDialogAsync("Create folder with *.json files", _lastOpenDirectory);

            if (string.IsNullOrEmpty(res) == false)
            {
                DirectoryTextBox.Text = res;
            }
        }
    }
}
