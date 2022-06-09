using System.IO;

namespace FootprintViewer.ViewModels.Settings
{
    public interface IFolderSourceInfo : ISourceInfo
    {
        string? Directory { get; set; }

        string? SearchPattern { get; set; }
    }

    public class FolderSourceInfo : IFolderSourceInfo
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

        public string? Directory { get; set; }

        public string? SearchPattern { get; set; }
    }
}
