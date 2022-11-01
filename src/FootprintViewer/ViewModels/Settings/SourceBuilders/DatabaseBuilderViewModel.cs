using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.DataManager.Sources;
using FootprintViewer.ViewModels.Dialogs;
using Npgsql;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.Settings.SourceBuilders;

public class DatabaseBuilderViewModel : DialogViewModelBase<ISource>
{
    private readonly ObservableAsPropertyHelper<bool> _isVerified;
    private readonly ReadOnlyObservableCollection<string> _availableTables;
    private readonly SourceList<string> _availableList = new();
    private readonly string _key;

    public DatabaseBuilderViewModel(string key)
    {
        _key = key;

        Host = "localhost";
        Port = 5432;
        Database = "FootprintViewerDatabase";
        Username = "postgres";
        Password = "user";

        Update = ReactiveCommand.Create<List<string>>(UpdateImpl, outputScheduler: RxApp.MainThreadScheduler);

        _availableList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _availableTables)
            .Subscribe();

        this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => SelectedTable = null);

        this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1.2))
            .Select(s => BuildConnectionString(s))
            .Select(s => IsConnectionValid(s) ? new List<string>(GetTablesNames(_key, s)) : new List<string>())
            .InvokeCommand(Update);

        _isVerified = AvailableTables
            .ToObservableChangeSet()
            .ToCollection()
            .Select(s => s.Count != 0)
            .ToProperty(this, x => x.IsVerified);

        var nextCommandCanExecute = this.WhenAnyValue(s => s.SelectedTable, (t) => !string.IsNullOrEmpty(t));

        // dialog
        EnableCancel = true;
        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back, CreateSource()), nextCommandCanExecute);
        CancelCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back));
    }

    private ISource CreateSource()
    {
        var str = extns2.ToConnectionString(Host!, Port, Database!, Username!, Password!);
        return new DatabaseSource(_key, str, SelectedTable!);
    }

    private void UpdateImpl(List<string> list)
    {
        _availableList.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    private static IEnumerable<string> GetTablesNames(string key, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var tableName = reader.GetString(0);

            if (DbHelper.TryValidateContext(key, connectionString, tableName) == true)
            {
                yield return tableName;
            }
        }
    }

    private static bool IsConnectionValid(string connectionString)
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

    private static string BuildConnectionString((string? host, int port, string? database, string? username, string? password) info)
    {
        return $"Host={info.host};Port={info.port};Database={info.database};Username={info.username};Password={info.password}";
    }

    private ReactiveCommand<List<string>, Unit> Update { get; }

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

    public ReadOnlyObservableCollection<string> AvailableTables => _availableTables;

    [Reactive]
    public string? SelectedTable { get; set; }

    public bool IsVerified => _isVerified.Value;
}
