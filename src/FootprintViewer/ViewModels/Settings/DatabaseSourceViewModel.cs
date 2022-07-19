using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FootprintViewer.Data.Sources;
using Npgsql;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface IDatabaseSourceViewModel : ISourceViewModel
    {
        string? Version { get; set; }

        string? Host { get; set; }

        int Port { get; set; }

        string? Database { get; set; }

        string? Username { get; set; }

        string? Password { get; set; }

        string? Table { get; set; }

        TableInfo? TableInfo { get; set; }
    }

    public class DatabaseSourceViewModel : ReactiveObject, IDatabaseSourceViewModel
    {
        public DatabaseSourceViewModel()
        {
            this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
                .Throttle(TimeSpan.FromSeconds(1.2))
                .Select(s => BuildConnectionString(s))
                .Select(s => IsConnectionValid(s) ? new List<string>(GetTablesNames(s)) : new List<string>())
                .ToPropertyEx(this, s => s.AvailableTables);
        }

        public DatabaseSourceViewModel(IDatabaseSource databaseSource) : this()
        {
            Version = databaseSource.Version;
            Host = databaseSource.Host;
            Port = databaseSource.Port;
            Database = databaseSource.Database;
            Username = databaseSource.Username;
            Password = databaseSource.Password;
            Table = databaseSource.Table;
        }

        private static string BuildConnectionString((string? host, int port, string? database, string? username, string? password) info)
        {
            return $"Host={info.host};Port={info.port};Database={info.database};Username={info.username};Password={info.password}";
        }

        private static bool IsConnectionValid(string? connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [ObservableAsProperty]
        public List<string> AvailableTables { get; } = new List<string>();

        private static IEnumerable<string> GetTablesNames(string? connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public string? Name => $"{Database}.{Table}";

        [Reactive]
        public TableInfo? TableInfo { get; set; }

        [Reactive]
        public string? Version { get; set; }

        [Reactive]
        public string? Host { get; set; }

        [Reactive]
        public int Port { get; set; }

        [Reactive]
        public string? Database { get; set; }

        [Reactive]
        public string? Username { get; set; }

        [Reactive]
        public string? Password { get; set; }

        [Reactive]
        public string? Table { get; set; }
    }
}
