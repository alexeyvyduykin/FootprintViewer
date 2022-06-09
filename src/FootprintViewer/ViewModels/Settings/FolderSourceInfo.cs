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

        public string? Name => System.IO.Path.GetDirectoryName(Directory);

        public string? Directory { get; set; }

        public string? SearchPattern { get; set; }
    }
}
