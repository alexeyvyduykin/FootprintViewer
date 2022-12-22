using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Items;
using FootprintViewer.ViewModels.TimelinePanel;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class FootprintTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly IMapNavigator _mapNavigator;
    private readonly SourceList<Footprint> _footprints = new();
    private readonly ReadOnlyObservableCollection<FootprintViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly ILayer? _layer;
    private readonly FootprintProvider _layerProvider;

    public FootprintTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        _mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        var map = dependencyResolver.GetExistingService<IMap>();
        _layer = map.GetLayer(LayerType.Footprint);
        _layerProvider = dependencyResolver.GetExistingService<FootprintProvider>();
        _featureManager = dependencyResolver.GetExistingService<FeatureManager>();
        var areaOfInterest = dependencyResolver.GetExistingService<AreaOfInterest>();
        var timelinePanel = dependencyResolver.GetExistingService<TimelinePanelViewModel>();

        Filter = new FootprintTabFilterViewModel(dependencyResolver);

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

        this.WhenAnyValue(s => s.ShowTimeline)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => timelinePanel.Show = s);

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
            .Where(s => s.Contains(DbKeys.Footprints.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        TargetToMap = ReactiveCommand.CreateFromTask<FootprintViewModel>(
            s => Task.Run(() =>
            {
                _mapNavigator.FlyToFootprint(s.Center);

                _featureManager
                    .OnLayer(_layer)
                    .Select(_layerProvider.Find(s.Name, "Name"));
            }), outputScheduler: RxApp.MainThreadScheduler);

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

    public ReactiveCommand<FootprintViewModel, Unit> TargetToMap { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<FootprintViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        var res = await _dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

        _footprints.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
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
    public bool ShowTimeline { get; set; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}
