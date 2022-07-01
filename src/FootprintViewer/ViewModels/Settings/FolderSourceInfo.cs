using System.IO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface IFolderSourceInfo : ISourceInfo
    {
        string? Directory { get; set; }

        string? SearchPattern { get; set; }
    }

    public class FolderSourceInfo : ReactiveObject, IFolderSourceInfo
    {
        public FolderSourceInfo()
        {

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
    }
}
