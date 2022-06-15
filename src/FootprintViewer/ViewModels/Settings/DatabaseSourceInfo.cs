using Npgsql;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.Serialization;

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

    [DataContract]
    public class DatabaseSourceInfo : ReactiveObject, IDatabaseSourceInfo
    {
        public DatabaseSourceInfo()
        {
            AvailableTables = new List<string>();

            this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
                .Throttle(TimeSpan.FromSeconds(1.2))
                .Subscribe(_ =>
                {
                    if (IsConnectionValid(DbOptions.BuildConnectionString(this)) == true)
                    {
                        AvailableTables = new List<string>(GetTablesNames(DbOptions.BuildConnectionString(this)));
                    }
                    else
                    {
                        AvailableTables = new List<string>();
                    }
                });

            this.WhenAnyValue(s => s.Table)
                .Subscribe(s =>
                {
                    IsValid = false;

                    if (string.IsNullOrEmpty(s) == false)
                    {
                        IsValid = true;
                    }
                });
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

        [DataMember]
        [Reactive]
        public string? Version { get; set; }

        [DataMember]
        [Reactive]
        public string? Host { get; set; }

        [DataMember]
        [Reactive]
        public int Port { get; set; }

        [DataMember]
        [Reactive]
        public string? Database { get; set; }

        [DataMember]
        [Reactive]
        public string? Username { get; set; }

        [DataMember]
        [Reactive]
        public string? Password { get; set; }

        [DataMember]
        [Reactive]
        public string? Table { get; set; }

        [Reactive]
        public List<string> AvailableTables { get; set; }

        [Reactive]
        public bool IsValid { get; set; }
    }
}
