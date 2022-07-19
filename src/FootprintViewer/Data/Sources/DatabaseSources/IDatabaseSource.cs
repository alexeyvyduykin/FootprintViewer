namespace FootprintViewer.Data.Sources
{
    public interface IDatabaseSource
    {
        string Version { get; init; }

        string Host { get; init; }

        int Port { get; init; }

        string Database { get; init; }

        string Username { get; init; }

        string Password { get; init; }

        string Table { get; init; }
    }
}
