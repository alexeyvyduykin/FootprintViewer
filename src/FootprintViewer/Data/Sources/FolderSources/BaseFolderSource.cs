namespace FootprintViewer.Data.Sources
{
    public abstract class BaseFolderSource : IFolderSource
    {
        public string Directory { get; init; } = string.Empty;

        public string SearchPattern { get; init; } = string.Empty;
    }
}
