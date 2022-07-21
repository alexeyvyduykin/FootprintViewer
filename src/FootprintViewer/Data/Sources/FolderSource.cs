namespace FootprintViewer.Data.Sources
{
    public class FolderSource : IFolderSource
    {
        public string Directory { get; init; } = string.Empty;

        public string SearchPattern { get; init; } = string.Empty;
    }
}
