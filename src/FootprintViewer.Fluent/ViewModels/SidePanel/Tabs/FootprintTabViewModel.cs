﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Extensions;
using FootprintViewer.Fluent.Services2;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Services;
using FootprintViewer.Styles;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed class FootprintTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMapService _mapService;
    private readonly SourceList<Footprint> _footprints = new();
    private readonly ReadOnlyObservableCollection<FootprintViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly FootprintProvider? _layerProvider;

    public FootprintTabViewModel()
    {
        Title = "Planned schedule viewer";

        Key = nameof(FootprintTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _mapService = Services.Locator.GetRequiredService<IMapService>();
        _layerProvider = _mapService.GetProvider<FootprintProvider>();
        _featureManager = Services.Locator.GetRequiredService<FeatureManager>();

        Filter = new FootprintTabFilterViewModel();
        Filter.SetAOIObservable(_mapService.AOI.Changed);

        var filter1 = Filter.AOIObservable;
        var filter2 = Filter.FilterObservable;
        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _footprints.Count);

        var filterObservable = _footprints
            .Connect()
            .Transform(s => new FootprintViewModel(s))
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Filter(filter1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(filter2)
            .Filter(filter3);

        filterObservable
            .Sort(SortExpressionComparer<FootprintViewModel>.Ascending(s => s.Begin))
            .Bind(out _items)
            .DisposeMany()
            .Subscribe(_ => FilteringItemCount = _items.Count);

        var layerObservable = filterObservable
            .Transform(s => s.Footprint!)
            .ToCollection();

        _layerProvider?.SetObservable(layerObservable);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        Enter = ReactiveCommand.Create<FootprintViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

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

        TargetToMap = ReactiveCommand.CreateFromTask<FootprintViewModel>(s => TargetToMapImpl(s), outputScheduler: RxApp.MainThreadScheduler);

        Observable.StartAsync(UpdateImpl);
    }

    private static Func<FootprintViewModel, bool> SearchStringPredicate(string? arg)
    {
        return (s =>
        {
            if (string.IsNullOrEmpty(arg) == true)
            {
                return true;
            }

            return s.Name.Contains(arg, StringComparison.CurrentCultureIgnoreCase);
        });
    }

    private async Task TargetToMapImpl(FootprintViewModel vm)
    {
        await Observable.Start(() => flyTo(vm), RxApp.MainThreadScheduler);

        void flyTo(FootprintViewModel vm)
        {
            _mapService.FlyToFootprint(vm.Center);

            _featureManager
                .OnLayer(_mapService.Map.GetLayer(LayerType.Footprint))
                .Select(_layerProvider?.Find(vm.Name, "Name"));
        }
    }

    public ReactiveCommand<FootprintViewModel, Unit> TargetToMap { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<FootprintViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        await Observable
            .Return(Unit.Default)
            .Delay(TimeSpan.FromSeconds(1));

        var ps = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            var footprints = ps
                .GetObservations()
                .Select(s => s.ToFootprint())
                .ToList();

            _footprints.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(footprints);
            });
        }
    }

    private void EnterImpl(FootprintViewModel footprint)
    {
        var name = footprint.Name;

        if (string.IsNullOrEmpty(name) == false)
        {
            _featureManager
                .OnLayer(_mapService.Map.GetLayer(LayerType.Footprint))
                .Enter(_layerProvider?.Find(name, "Name"));
        }
    }

    private void LeaveImpl()
    {
        _featureManager
            .OnLayer(_mapService.Map.GetLayer(LayerType.Footprint))
            .Leave();
    }

    public FootprintViewModel? GetFootprintViewModel(string name)
    {
        return Items.Where(s => s.Name.Equals(name)).FirstOrDefault();
    }

    public IAOIFilter<FootprintViewModel> Filter { get; }

    public ReadOnlyObservableCollection<FootprintViewModel> Items => _items;

    [Reactive]
    public string? SearchString { get; set; }

    [Reactive]
    public bool IsFilterOnMap { get; set; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}