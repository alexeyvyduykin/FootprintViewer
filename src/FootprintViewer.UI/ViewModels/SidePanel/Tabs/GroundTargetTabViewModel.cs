﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Models;
using FootprintViewer.Services;
using FootprintViewer.UI.Extensions;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.SidePanel.Filters;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.SidePanel.Tabs;

public sealed class GroundTargetTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMapService _mapService;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public GroundTargetTabViewModel()
    {
        Title = "Ground targets viewer";

        Key = nameof(GroundTargetTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _mapService = Services.Locator.GetRequiredService<IMapService>();

        Filter = new GroundTargetTabFilterViewModel();
        Filter.SetAOIObservable(_mapService.AOI.Changed);

        var filter1 = Filter.AOIObservable;
        var filter2 = Filter.FilterObservable;
        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _groundTargets.Count);

        var filterObservable = _groundTargets
            .Connect()
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .ObserveOn(RxApp.MainThreadScheduler);

        filterObservable
            .Sort(SortExpressionComparer<GroundTargetViewModel>.Ascending(t => t.Name))
            .Bind(out _items)
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        var layerObservable = filterObservable
            .Transform(s => s.GroundTarget)
            .ToCollection();

        var layerProvider = _mapService.GetProvider<GroundTargetProvider>();
        layerProvider?.SetObservable(layerObservable);

        _localStorage
            .PlannedScheduleObservable
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.SelectedItem)
            .WhereNotNull()
            .Subscribe(s => _mapService.SelectFeature(s.Name, LayerType.GroundTarget));

        Enter = ReactiveCommand.Create<GroundTargetViewModel>(s => _mapService.EnterFeature(s.Name, LayerType.GroundTarget));

        Leave = ReactiveCommand.Create(() => _mapService.LeaveFeature(LayerType.GroundTarget));

        _isLoading = Update.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        Observable.StartAsync(UpdateImpl);
    }

    public IAOIFilter<GroundTargetViewModel> Filter { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<GroundTargetViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    private async Task UpdateImpl()
    {
        var storage = Services.Locator.GetRequiredService<ILocalStorageService>();

        var ps = (await storage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            _groundTargets.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(ps.GroundTargets);
            });
        }
    }

    private static Func<GroundTargetViewModel, bool> SearchStringPredicate(string? arg)
    {
        return (s =>
        {
            if (string.IsNullOrEmpty(arg) == true)
            {
                return true;
            }

            return s.Name?.Contains(arg, StringComparison.CurrentCultureIgnoreCase) ?? true;
        });
    }

    public bool IsLoading => _isLoading.Value;

    public ReadOnlyObservableCollection<GroundTargetViewModel> Items => _items;

    [Reactive]
    public bool IsFilterOnMap { get; set; }

    [Reactive]
    public string? SearchString { get; set; }

    [Reactive]
    public GroundTargetViewModel? SelectedItem { get; set; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}