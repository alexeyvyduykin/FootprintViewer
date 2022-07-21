namespace FootprintViewer.Data
{
    public interface IDatabaseSource : IDataSource
    {
        string Version { get; }

        string Host { get; }

        int Port { get; }

        string Database { get; }

        string Username { get; }

        string Password { get; }

        string Table { get; }
    }
}
