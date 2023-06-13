using DynamicData;
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

namespace PlannedScheduleOnMapSample.ViewModels;

public class GroundTargetTabViewModel : ViewModelBase
{
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private IObservable<IReadOnlyCollection<GroundTarget>> _layerObservable;
    private readonly DataManager _dataManager;

    public GroundTargetTabViewModel() : this(DesignData.CreateDataManager()) { }

    public GroundTargetTabViewModel(DataManager dataManager)
    {
        _dataManager = dataManager;

        var observable = _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler);

        observable
            .Transform(s => new GroundTargetViewModel(s))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        _layerObservable = observable
            .ToCollection();

        Update = ReactiveCommand.CreateFromTask<(bool, bool)>(s => UpdateImpl(s.Item1, s.Item2));

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.OnlyObservables, s => s.NotObservables)
            .InvokeCommand(Update);
    }

    public void ToLayerProvider(GroundTargetProvider provider)
    {
        provider.SetObservable(_layerObservable);
    }

    public ReactiveCommand<(bool, bool), Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl(bool onlyObservables, bool notObservables)
    {
        var res = new List<GroundTarget>();

        if (onlyObservables == true || notObservables == true)
        {
            var res2 = await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey);

            var tasks = res2.FirstOrDefault()!.PlannedSchedules
                .Cast<ObservationTaskResult>();

            var gts = res2.FirstOrDefault()!.GroundTargets;

            foreach (var item in gts)
            {
                if (onlyObservables == true && notObservables == true)
                {
                    res.Add(item);
                }
                else
                {
                    var res3 = tasks.Select(s => s.TargetName).Contains(item.Name);

                    if (res3 == true && onlyObservables == true)
                    {
                        res.Add(item);
                    }
                    else if (res3 == false && notObservables == true)
                    {
                        res.Add(item);
                    }
                }
            }
        }

        _groundTargets.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
    }

    [Reactive]
    public bool OnlyObservables { get; set; }

    [Reactive]
    public bool NotObservables { get; set; }

    public ReadOnlyObservableCollection<GroundTargetViewModel> Items => _items;
}
