using Microsoft.EntityFrameworkCore;
using System;

namespace FootprintViewer.Data.Sources
{
    public abstract class BaseDatabaseSource<T> : IDatabaseSource where T : DbCustomContext
    {
        private DbContextOptions<T>? _options;

        public DbContextOptions<T> Options =>
            _options ??= BuildDbContextOptions<T>(Version, Host, Port, Database, Username, Password);

        protected static string ToConnectionString(string host, int port, string database, string username, string password)
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        protected static DbContextOptions<T> BuildDbContextOptions<T>(string version, string host, int port, string database, string username, string password) where T : DbCustomContext
        {
            var connectionString = ToConnectionString(host, port, database, username, password);
            var res = version!.Split(new[] { '.' });
            var major = int.Parse(res[0]);
            var minor = int.Parse(res[1]);

            var optionsBuilder = new DbContextOptionsBuilder<T>();

            var options = optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.SetPostgresVersion(new Version(major, minor));
                options.UseNetTopologySuite();
            }).Options;

            return options;
        }

        public string Version { get; init; } = string.Empty;

        public string Host { get; init; } = string.Empty;

        public int Port { get; init; }

        public string Database { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public string Table { get; init; } = string.Empty;
    }
}
