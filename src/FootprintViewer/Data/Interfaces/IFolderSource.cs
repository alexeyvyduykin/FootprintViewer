namespace FootprintViewer.Data
{
    public interface IFolderSource : IDataSource
    {
        string Directory { get; }

        string SearchPattern { get; }
    }
}
