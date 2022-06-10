namespace FootprintViewer.ViewModels
{
    public interface IDatabaseSourceInfo : ISourceInfo
    {
        string? Version { get; set; }

        string? Host { get; set; }

        int Port { get; set; }

        string? Database { get; set; }

        string? Username { get; set; }

        string? Password { get; set; }

        string? Table { get; set; }
    }

    public class DatabaseSourceInfo : IDatabaseSourceInfo
    {
        public DatabaseSourceInfo()
        {

        }

        public string? Name => $"{Database}.{Table}";

        public string? Version { get; set; }

        public string? Host { get; set; }

        public int Port { get; set; }

        public string? Database { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Table { get; set; }
    }
}
