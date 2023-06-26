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
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.ViewModels;

public class PlannedScheduleTabViewModel : ViewModelBase
{
    private readonly SourceList<ITaskResult> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private IObservable<IReadOnlyCollection<Footprint>> _layerObservable;
    private readonly DataManager _dataManager;
    private readonly Subject<PlannedScheduleResult> _subj = new();

    public PlannedScheduleTabViewModel() : this(DesignData.CreateDataManager()) { }

    public PlannedScheduleTabViewModel(DataManager dataManager)
    {
        _dataManager = dataManager;

        var filter1 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        var observable = _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s))
            .Filter(filter1);

        observable
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
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
            .Subscribe(s =>
            {
                var name = $"Footprint_{s.TaskName}";
                MainWindowViewModel.Instance.SelectFootprint(name, s, IsPreview, IsFullTrack, IsSwath, IsGroundTarget);
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

            if (arg.Contains("Satellite:") == true)
            {
                var res = arg.Replace("Satellite:", "").Trim();

                return s.SatelliteName.Contains(res, StringComparison.CurrentCultureIgnoreCase);
            }

            return s.TaskName.Contains(arg, StringComparison.CurrentCultureIgnoreCase);
        });
    }

    public void ToLayerProvider(FootprintProvider provider)
    {
        provider.SetObservable(_layerObservable);
    }

    public void ToLayerProvider(FootprintTrackProvider provider)
    {
        var layerObservable = _subj.AsObservable();
     
        provider.SetObservable(layerObservable);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    // TODO: DblClick on list item call CenterOn
    public ReactiveCommand<TaskResultViewModel, Unit> CenterOn { get; }

    public ReactiveCommand<TaskResultViewModel, Unit> Entered { get; }

    public ReactiveCommand<Unit, Unit> Exited { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey);

        var tasks = res.FirstOrDefault()!.PlannedSchedules;

        _subj.OnNext(res.First());

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
    public bool IsFullTrack { get; set; }

    [Reactive]
    public bool IsSwath { get; set; }

    [Reactive]
    public bool IsGroundTarget { get; set; }

    [Reactive]
    public bool IsObservables { get; set; }

    [Reactive]
    public bool IsAvailabilities { get; set; }

    [Reactive]
    public bool IsDimming { get; set; }

    [Reactive]
    public TaskResultViewModel? SelectedItem { get; set; }
}
