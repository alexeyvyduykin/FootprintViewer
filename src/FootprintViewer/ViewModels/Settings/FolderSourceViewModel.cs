using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.IO;

namespace FootprintViewer.ViewModels
{
    public interface IFolderSourceViewModel : ISourceViewModel
    {
        string? Directory { get; set; }

        string? SearchPattern { get; set; }
    }

    public class FolderSourceViewModel : ReactiveObject, IFolderSourceViewModel
    {
        public FolderSourceViewModel()
        {

        }

        public FolderSourceViewModel(IFolderSource folderSource) : this()
        {
            Directory = folderSource.Directory;
            SearchPattern = folderSource.SearchPattern;
        }

        private static string GetName(string directory)
        {
            string[] res = directory.Split(Path.DirectorySeparatorChar);

            if (res.Length > 1)
            {
                return $"{res[^2]}/{res[^1]}";
            }

            if (res.Length > 0)
            {
                return res[^1];
            }

            return string.Empty;
        }

        public string? Name => Directory != null ? GetName(Directory) : string.Empty;

        [Reactive]
        public string? Directory { get; set; }

        public string? SearchPattern { get; set; }

        string? ISourceViewModel.Name { get; set; }

        public ISource Source => throw new System.NotImplementedException();
    }
}
