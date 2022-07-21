namespace FootprintViewer.Data
{
    public interface IRandomSource : IDataSource
    {
        string Name { get; }
    }
}
