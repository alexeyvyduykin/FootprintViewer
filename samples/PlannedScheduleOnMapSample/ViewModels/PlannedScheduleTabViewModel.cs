using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using PlannedScheduleOnMapSample.Design;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.ViewModels.Items;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
    private IObservable<IReadOnlyCollection<Footprint>> _layerObservable;
    private readonly DataManager _dataManager;

    public PlannedScheduleTabViewModel() : this(DesignData.CreateDataManager()) { }

    public PlannedScheduleTabViewModel(DataManager dataManager)
    {
        _dataManager = dataManager;

        var observable = _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s));

        observable
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _layerObservable = observable
            .Transform(s => TaskResultViewModel.Create(s.Model))
            .ToCollection();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Update.Execute().Subscribe();
    }

    public void ToLayerProvider(FootprintProvider provider)
    {
        provider.SetObservable(_layerObservable);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey);

        var tasks = res.FirstOrDefault()!.PlannedSchedules;

        _plannedSchedules.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(tasks);
        });
    }

    public ReadOnlyObservableCollection<TaskResultViewModel> Items => _items;
}
