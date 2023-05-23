using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Models;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class SelectRecordPageViewModel : RoutableViewModel
{
    private readonly SourceList<PlannedScheduleResult> _sourceList = new();
    private readonly ReadOnlyObservableCollection<PlannedScheduleItemViewModel> _items;
    private readonly Func<PlannedScheduleDbContext> _contextCreator;

    public SelectRecordPageViewModel(Func<PlannedScheduleDbContext> contextCreator)
    {
        _contextCreator = contextCreator;

        EnableBack = true;
        EnableCancel = true;

        _sourceList
            .Connect()
            .Transform(s => new PlannedScheduleItemViewModel(s.Name, s.DateTime))
            .Sort(SortExpressionComparer<PlannedScheduleItemViewModel>.Ascending(t => t.DateTime))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        var nextCommandCanExecute = this.WhenAnyValue(s => s.SelectedItem, selector: (s) => s is { });

        NextCommand = ReactiveCommand.Create(OnNext, nextCommandCanExecute);

        RxApp.MainThreadScheduler.Schedule(async () =>
        {
            using var context = _contextCreator.Invoke();

            var list = await context.GetValuesAsync();

            var res = list.Where(s => s is PlannedScheduleResult).Cast<PlannedScheduleResult>().ToList();

            _sourceList.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(res);
            });
        });
    }

    public override string Title { get => "Select planned schedule"; protected set { } }

    public ReadOnlyObservableCollection<PlannedScheduleItemViewModel> Items => _items;

    [Reactive]
    public PlannedScheduleItemViewModel? SelectedItem { get; set; }

    private void OnNext()
    {
        Navigate().Clear();

        Services.Locator.GetRequiredService<IDataManager>().UnregisterSources(DbKeys.PlannedSchedules.ToString());

        foreach (var (key, source) in Global.CreateSources(_contextCreator))
        {
            Services.Locator.GetRequiredService<IDataManager>().RegisterSource(key, source);
        }

        Services.Locator.GetRequiredService<IDataManager>().UpdateData();

        Save();
    }

    private void Save()
    {
        var config = new Config(Services.Config.FilePath);

        RxApp.MainThreadScheduler.Schedule(
            () =>
            {
                try
                {
                    config.LoadFile();
                    EditConfigOnSave(config);
                    config.ToFile();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex);
                }
            });
    }

    private void EditConfigOnSave(Config config)
    {
        using var context = _contextCreator.Invoke();

        config.PlannedScheduleState = PlannedScheduleState.Database;

        config.LastPlannedScheduleConnection = new DatabaseConnection()
        {
            ConnectionString = context.ConnectionString,
            TableName = context.TableName,
            RecordName = SelectedItem!.Name,
        };
    }
}
