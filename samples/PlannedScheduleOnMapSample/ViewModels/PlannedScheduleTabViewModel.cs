using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.Models;
using FootprintViewer.Helpers;
using PlannedScheduleOnMapSample.ViewModels.Items;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.ViewModels;

public class PlannedScheduleTabViewModel : ViewModelBase
{
    private readonly SourceList<ITaskResult> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public PlannedScheduleTabViewModel()
    {
        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s))
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Update.Execute().Subscribe();
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var list = await Observable.Start(() =>
        {
            string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "PlannedSchedule.json");

            var result = (PlannedScheduleResult)JsonHelpers.DeserializeFromFile<PlannedScheduleResult>(path)!;

            return result.PlannedSchedules.ToList();
        }, RxApp.TaskpoolScheduler);

        _plannedSchedules.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    public ReadOnlyObservableCollection<TaskResultViewModel> Items => _items;
}
