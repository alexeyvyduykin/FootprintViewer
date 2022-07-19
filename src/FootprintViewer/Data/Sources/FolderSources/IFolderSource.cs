namespace FootprintViewer.Data.Sources
{
    public interface IFolderSource
    {
        string Directory { get; }

        string SearchPattern { get; }
    }
}
