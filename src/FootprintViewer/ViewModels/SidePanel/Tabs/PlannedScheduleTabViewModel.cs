using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Data.Models;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public sealed class PlannedScheduleTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<(string, ITaskResult)> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly IMapNavigator _mapNavigator;
    private readonly FeatureManager _featureManager;
    private readonly FootprintProvider _layerProvider;
    private readonly ILayer? _layer;

    public PlannedScheduleTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        _layerProvider = dependencyResolver.GetExistingService<FootprintProvider>();
        _featureManager = dependencyResolver.GetExistingService<FeatureManager>();
        _mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        _layer = dependencyResolver.GetExistingService<IMap>().GetLayer(LayerType.Footprint);

        Title = "Просмотр рабочей программы";

        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _plannedSchedules.Count);

        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s.Item1, s.Item2))
            .Sort(SortExpressionComparer<TaskResultViewModel>.Ascending(s => s.Begin))
            .Bind(out _items)
            .DisposeMany()
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        TargetToMap = ReactiveCommand.CreateFromTask<ObservationTaskResult>(s => TargetToMapImpl(s), outputScheduler: RxApp.MainThreadScheduler);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Footprints.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        await Observable
            .Return(Unit.Default)
            .Delay(TimeSpan.FromSeconds(1));

        var result = (await _dataManager.GetDataAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (result != null)
        {
            var list = result.PlannedSchedules.SelectMany(s => s.Value.Select(task => (s.Key, task))).ToList();

            _plannedSchedules.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }
    }

    private async Task TargetToMapImpl(ObservationTaskResult result)
    {
        await Observable.Start(() => flyTo(result), RxApp.MainThreadScheduler);

        void flyTo(ObservationTaskResult result)
        {
            _mapNavigator.FlyToFootprint(result.Footprint.Center.Coordinate);

            //_featureManager
            //    .OnLayer(_layer)
            //    .Select(_layerProvider.Find(vm.Name, "Name"));            
        }
    }

    public ReadOnlyObservableCollection<TaskResultViewModel> Items => _items;

    public ReactiveCommand<ObservationTaskResult, Unit> TargetToMap { get; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}
