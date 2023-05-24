﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Services;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed partial class PlannedScheduleTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly SourceList<ITaskResult> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly IMapNavigator _mapNavigator;
    private readonly FeatureManager _featureManager;
    private readonly FootprintProvider _layerProvider;
    private readonly ILayer? _layer;

    public PlannedScheduleTabViewModel()
    {
        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _layerProvider = Services.Locator.GetRequiredService<FootprintProvider>();
        _featureManager = Services.Locator.GetRequiredService<FeatureManager>();
        _mapNavigator = Services.Locator.GetRequiredService<MapNavigator>();
        _layer = Services.Locator.GetRequiredService<Map>().GetLayer(LayerType.Footprint);

        Title = "Просмотр рабочей программы";
        Key = nameof(PlannedScheduleTabViewModel);

        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _plannedSchedules.Count);

        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s))
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

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
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

        var result = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (result != null)
        {
            var list = result.PlannedSchedules.ToList();

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
            _mapNavigator.FlyToFootprint(result.Geometry.Center.Coordinate);

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

public partial class PlannedScheduleTabViewModel
{
    public PlannedScheduleTabViewModel(DesignDataDependencyResolver resolver)
    {
        _localStorage = resolver.GetService<ILocalStorageService>();
        _layerProvider = resolver.GetService<FootprintProvider>();
        _featureManager = resolver.GetService<FeatureManager>();
        _mapNavigator = resolver.GetService<IMapNavigator>();
        _layer = resolver.GetService<IMap>().GetLayer(LayerType.Footprint);

        Title = "Просмотр рабочей программы";
        Key = nameof(PlannedScheduleTabViewModel);
        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _plannedSchedules.Count);

        _plannedSchedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new TaskResultViewModel(s))
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

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);
    }
}