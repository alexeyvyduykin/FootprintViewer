using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
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

public sealed class GroundTargetTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly ILayer? _layer;
    private readonly GroundTargetProvider _layerProvider;

    public GroundTargetTabViewModel()
    {
        Title = "Ground targets viewer";

        Key = nameof(GroundTargetTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var map = Services.Locator.GetRequiredService<Map>();
        _layer = map.GetLayer(LayerType.GroundTarget);
        _layerProvider = Services.Locator.GetRequiredService<GroundTargetProvider>();
        _featureManager = Services.Locator.GetRequiredService<FeatureManager>();
        var areaOfInterest = Services.Locator.GetRequiredService<AreaOfInterest>();

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

        var filterObservable = _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(filter1)
            .Filter(filter2)
            .Filter(filter3);

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

        _layerProvider.SetObservable(layerObservable);

        _localStorage
            .PlannedScheduleObservable
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