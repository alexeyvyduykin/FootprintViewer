namespace FootprintViewer.Data
{
    public interface IFileSource : IDataSource
    {
        string Path { get; }
    }
}
