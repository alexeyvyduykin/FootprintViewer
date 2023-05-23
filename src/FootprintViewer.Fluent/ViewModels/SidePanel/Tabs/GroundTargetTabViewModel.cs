using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed partial class GroundTargetTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly ILayer? _layer;
    private readonly GroundTargetProvider _layerProvider;

    public GroundTargetTabViewModel()
    {
        _dataManager = Services.Locator.GetRequiredService<IDataManager>();
        var map = Services.Locator.GetRequiredService<Map>();
        _layer = map.GetLayer(LayerType.GroundTarget);
        _layerProvider = Services.Locator.GetRequiredService<GroundTargetProvider>();
        _featureManager = Services.Locator.GetRequiredService<FeatureManager>();
        var areaOfInterest = Services.Locator.GetRequiredService<AreaOfInterest>();

        Title = "Просмотр наземных целей";
        Key = nameof(GroundTargetTabViewModel);

        Filter = new GroundTargetTabFilterViewModel();

        var filter1 = Filter.AOIFilterObservable;
        var filter2 = Filter.FilterObservable;

        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _groundTargets.Count);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .Sort(SortExpressionComparer<GroundTargetViewModel>.Ascending(t => t.Name))
            .Bind(out _items)
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        this.WhenAnyValue(s => s.IsFilterOnMap)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => IsFilterOnMapChanged());

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .ToCollection()
            .Subscribe(UpdateProvider);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.SelectedItem)
            .InvokeCommand(ReactiveCommand.Create<GroundTargetViewModel?>(SelectImpl));

        Enter = ReactiveCommand.Create<GroundTargetViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _isLoading = Update.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        areaOfInterest.AOIChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => Filter.AOI = s);
    }

    public IAOIFilter<GroundTargetViewModel> Filter { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<GroundTargetViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    private async Task UpdateImpl()
    {
        var ps = (await _dataManager.GetDataAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            _groundTargets.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(ps.GroundTargets);
            });
        }
    }

    private void EnterImpl(GroundTargetViewModel groundTarget)
    {
        var name = groundTarget.Name;

        if (string.IsNullOrEmpty(name) == false)
        {
            _featureManager
                .OnLayer(_layer)
                .Enter(_layerProvider.Find(name, "Name"));
        }
    }

    private void LeaveImpl()
    {
        _featureManager
            .OnLayer(_layer)
            .Leave();
    }

    private void SelectImpl(GroundTargetViewModel? groundTarget)
    {
        if (groundTarget != null)
        {
            var name = groundTarget.Name;

            if (string.IsNullOrEmpty(name) == false)
            {
                _featureManager
                    .OnLayer(_layer)
                    .Select(_layerProvider.Find(name, "Name"));
            }
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

    private void IsFilterOnMapChanged()
    {
        if (IsFilterOnMap == true)
        {
            var res = Items
                .Select(s => s.GroundTarget)
                .ToList();

            _layerProvider.UpdateData(res);
        }
        else
        {
            _layerProvider.Update.Execute().Subscribe();
        }
    }

    private void UpdateProvider(IReadOnlyCollection<GroundTargetViewModel> groundTarget)
    {
        if (IsFilterOnMap == true)
        {
            var res = groundTarget
                .Select(s => s.GroundTarget)
                .ToList();

            _layerProvider.UpdateData(res);
        }
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

public partial class GroundTargetTabViewModel
{
    public GroundTargetTabViewModel(DesignDataDependencyResolver resolver)
    {
        _dataManager = resolver.GetService<IDataManager>();
        _layer = resolver.GetService<IMap>().GetLayer(LayerType.GroundTarget);
        _layerProvider = resolver.GetService<GroundTargetProvider>();
        _featureManager = resolver.GetService<FeatureManager>();
        var areaOfInterest = resolver.GetService<AreaOfInterest>();

        Title = "Просмотр наземных целей";
        Key = nameof(GroundTargetTabViewModel);
        Filter = new GroundTargetTabFilterViewModel();

        var filter1 = Filter.AOIFilterObservable;
        var filter2 = Filter.FilterObservable;

        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _groundTargets.Count);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .Sort(SortExpressionComparer<GroundTargetViewModel>.Ascending(t => t.Name))
            .Bind(out _items)
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        this.WhenAnyValue(s => s.IsFilterOnMap)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => IsFilterOnMapChanged());

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .ToCollection()
            .Subscribe(UpdateProvider);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.SelectedItem)
            .InvokeCommand(ReactiveCommand.Create<GroundTargetViewModel?>(SelectImpl));

        Enter = ReactiveCommand.Create<GroundTargetViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _isLoading = Update.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        areaOfInterest.AOIChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => Filter.AOI = s);
    }
}