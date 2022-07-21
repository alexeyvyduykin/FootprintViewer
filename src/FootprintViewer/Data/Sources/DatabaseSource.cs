namespace FootprintViewer.Data.Sources
{
    public class DatabaseSource : IDatabaseSource
    {
        public string Version { get; init; } = string.Empty;

        public string Host { get; init; } = string.Empty;

        public int Port { get; init; }

        public string Database { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public string Table { get; init; } = string.Empty;
    }
}
