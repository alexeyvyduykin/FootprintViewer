using System;

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

        public bool Equals(IDataSource? other)
        {
            if (other is IDatabaseSource source)
            {
                if (source.Version == Version &&
                   source.Host == Host &&
                   source.Database == Database &&
                   source.Username == Username &&
                   source.Password == Password &&
                   source.Table == Table)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
