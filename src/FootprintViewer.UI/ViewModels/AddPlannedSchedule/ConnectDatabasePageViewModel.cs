using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.UI.ViewModels.Navigation;
using Npgsql;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.AddPlannedSchedule;

public class ConnectDatabasePageViewModel : RoutableViewModel
{
    private readonly ObservableAsPropertyHelper<bool> _isVerified;
    private readonly ReadOnlyObservableCollection<string> _availableTables;
    private readonly SourceList<string> _availableList = new();

    private static string? _lastHost = "localhost";
    private static string? _lastDatabase = "FootprintViewerDatabase";
    private static string? _lastUsername = "postgres";
    private static string? _lastPassword = "user";

    public ConnectDatabasePageViewModel()
    {
        EnableBack = true;
        EnableCancel = true;

        Update = ReactiveCommand.Create<List<string>>(UpdateImpl, outputScheduler: RxApp.MainThreadScheduler);

        _availableList.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _availableTables)
            .Subscribe();

        this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                SelectedTable = null;
                _lastHost = Host;
                _lastDatabase = Database;
                _lastUsername = Username;
                _lastPassword = Password;
            });

        this.WhenAnyValue(s => s.Host, s => s.Port, s => s.Database, s => s.Username, s => s.Password)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1.2))
            .Select(s => BuildConnectionString(s))
            .Select(s => IsConnectionValid(s) ? new List<string>(GetTablesNames(s)) : new List<string>())
            .InvokeCommand(Update);

        _isVerified = AvailableTables
            .ToObservableChangeSet()
            .ToCollection()
            .Select(s => s.Count != 0)
            .ToProperty(this, x => x.IsVerified);

        var nextCommandCanExecute = this.WhenAnyValue(s => s.SelectedTable, (t) => !string.IsNullOrEmpty(t));

        NextCommand = ReactiveCommand.Create(OnNext, nextCommandCanExecute);
    }

    private ReactiveCommand<List<string>, Unit> Update { get; }

    public override string Title { get => "Connect database"; protected set { } }

    [Reactive]
    public string? Host { get; set; } = _lastHost;

    [Reactive]
    public int Port { get; set; } = 5432;

    [Reactive]
    public string? Database { get; set; } = _lastDatabase;

    [Reactive]
    public string? Username { get; set; } = _lastUsername;

    [Reactive]
    public string? Password { get; set; } = _lastPassword;

    [Reactive]
    public string? SelectedTable { get; set; }

    public ReadOnlyObservableCollection<string> AvailableTables => _availableTables;

    public bool IsVerified => _isVerified.Value;

    private void OnNext()
    {
        Navigate().To(new SelectRecordPageViewModel(() => CreateContext()));
    }

    private PlannedScheduleDbContext CreateContext()
    {
        var str = ConnectionString.Build(Host!, Port, Database!, Username!, Password!).ToString();

        return new PlannedScheduleDbContext(SelectedTable!, str);
    }

    private void UpdateImpl(List<string> list)
    {
        _availableList.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    private IEnumerable<string> GetTablesNames(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var command = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var tableName = reader.GetString(0);

            if (DbHelpers.TryValidateContext(DbKeys.PlannedSchedules, () => new PlannedScheduleDbContext(tableName, connectionString)) == true)
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
}