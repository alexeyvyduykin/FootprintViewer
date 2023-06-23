using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using PlannedScheduleOnMapSample.Design;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.ViewModels.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
 
        var filter1 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        observable
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
            .Filter(filter1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _layerObservable = observable
            .Transform(s => TaskResultViewModel.Create(s.Model))
            .ToCollection();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);
       
        CenterOn = ReactiveCommand.Create<TaskResultViewModel>(CenterOnImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Update.Execute().Subscribe();

        this.WhenAnyValue(s => s.SelectedItem)
            .WhereNotNull()
            .Select(s => s.Model)
            .Subscribe(async s => 
            {
                var name = $"Footprint_{s.TaskName}";               
               // MainWindowViewModel.Instance.SelectFootprint(name);
                await MainWindowViewModel.Instance.SelectFootprint(name, IsPreview);
                //MainWindowViewModel.Instance.FlyToFootprint(name);
            });

        this.WhenAnyValue(s => s.IsDimming)
            .Skip(1)
            .Subscribe(s => 
            {
                MainWindowViewModel.Instance.FootprintDimming(s); 
            });

        Entered = ReactiveCommand.Create<TaskResultViewModel>(s => 
        {
            var name = $"Footprint_{s.TaskName}";

            MainWindowViewModel.Instance.EnterFootprint(name);
        });

        Exited = ReactiveCommand.Create(() => 
        {
            MainWindowViewModel.Instance.LeaveFootprint();
        });
    }

    private static Func<TaskResultViewModel, bool> SearchStringPredicate(string? arg)
    {
        return (s =>
        {
            if (string.IsNullOrEmpty(arg) == true)
            {
                return true;
            }

            return s.TaskName.Contains(arg, StringComparison.CurrentCultureIgnoreCase);
        });
    }

    public void ToLayerProvider(FootprintProvider provider)
    {
        provider.SetObservable(_layerObservable);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<TaskResultViewModel, Unit> CenterOn { get; }

    public ReactiveCommand<TaskResultViewModel, Unit> Entered { get; }

    public ReactiveCommand<Unit, Unit> Exited { get; }

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

    private void CenterOnImpl(TaskResultViewModel task)
    {
        var name = $"Footprint_{task.TaskName}";

        MainWindowViewModel.Instance.FlyToFootprint(name);
    }

    public ReadOnlyObservableCollection<TaskResultViewModel> Items => _items;

    [Reactive]
    public string? SearchString { get; set; }

    [Reactive]
    public bool IsPreview { get; set; }

    [Reactive]
    public bool IsDimming { get; set; }

    [Reactive]
    public TaskResultViewModel? SelectedItem { get; set; }
}
