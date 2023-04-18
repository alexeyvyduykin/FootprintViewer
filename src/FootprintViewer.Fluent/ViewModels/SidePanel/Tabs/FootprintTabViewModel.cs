using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
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

public sealed class FootprintTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly IMapNavigator _mapNavigator;
    private readonly SourceList<Footprint> _footprints = new();
    private readonly ReadOnlyObservableCollection<FootprintViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly ILayer? _layer;
    private readonly FootprintProvider _layerProvider;

    public FootprintTabViewModel()
    {
        _dataManager = Services.DataManager;
        _mapNavigator = Services.MapNavigator;
        var map = Services.Map;
        _layer = map.GetLayer(LayerType.Footprint);
        _layerProvider = Services.FootprintProvider;
        _featureManager = Services.FeatureManager;
        var areaOfInterest = Services.AreaOfInterest;

        Filter = new FootprintTabFilterViewModel();

        Title = "Просмотр рабочей программы";

        var filter1 = Filter.AOIFilterObservable;
        var filter2 = Filter.FilterObservable;

        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _footprints.Count);

        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new FootprintViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .Sort(SortExpressionComparer<FootprintViewModel>.Ascending(s => s.Begin))
            .Bind(out _items)
            .DisposeMany()
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        this.WhenAnyValue(s => s.IsFilterOnMap)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => IsFilterOnMapChanged());

        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new FootprintViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3)
            .ToCollection()
            .Subscribe(UpdateProvider);

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        Enter = ReactiveCommand.Create<FootprintViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        EmptySearchString = ReactiveCommand.Create(() => { SearchString = string.Empty; }, outputScheduler: RxApp.MainThreadScheduler);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        _dataManager.DataChanged
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

        areaOfInterest.AOIChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => Filter.AOI = s);
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
            _mapNavigator.FlyToFootprint(vm.Center);

            _featureManager
                .OnLayer(_layer)
                .Select(_layerProvider.Find(vm.Name, "Name"));
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

        var ps = (await _dataManager.GetDataAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

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

    private void UpdateProvider(IReadOnlyCollection<FootprintViewModel> footprints)
    {
        if (IsFilterOnMap == true)
        {
            var res = footprints
                .Where(s => s.Footprint != null)
                .Select(s => s.Footprint!)
                .ToList();

            _layerProvider.UpdateData(res);
        }
    }

    private void IsFilterOnMapChanged()
    {
        if (IsFilterOnMap == true)
        {
            var res = Items.Where(s => s.Footprint != null)
                .Select(s => s.Footprint!)
                .ToList();

            _layerProvider.UpdateData(res);
        }
        else
        {
            _layerProvider.Update.Execute().Subscribe();
        }
    }

    public FootprintViewModel? GetFootprintViewModel(string name)
    {
        return Items.Where(s => s.Name.Equals(name)).FirstOrDefault();
    }

    public IAOIFilter<FootprintViewModel> Filter { get; }

    public ReadOnlyObservableCollection<FootprintViewModel> Items => _items;

    [Reactive]
    public string? SearchString { get; set; }

    public ReactiveCommand<Unit, Unit> EmptySearchString { get; }

    [Reactive]
    public bool IsFilterOnMap { get; set; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}
