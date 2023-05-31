using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.UI.Extensions;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using FootprintViewer.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.SidePanel.Tabs;

public sealed class PlannedScheduleTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMapService _mapService;
    private readonly SourceList<ITaskResult> _plannedSchedules = new();
    private readonly ReadOnlyObservableCollection<TaskResultViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public PlannedScheduleTabViewModel()
    {
        Title = "Planned schedule viewer";

        Key = nameof(PlannedScheduleTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _mapService = Services.Locator.GetRequiredService<IMapService>();

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
            _mapService.FlyToFootprint(result.Geometry.Center.Coordinate);
            //_mapService.SelectFeature(result.Name, LayerType.Footprint);
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