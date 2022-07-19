namespace FootprintViewer.Data.Sources
{
    public interface IDatabaseSource
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
