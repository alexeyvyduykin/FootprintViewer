using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Services;
using FootprintViewer.Styles;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;

public sealed partial class GroundStationTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly LayerStyleManager _layerStyleManager;
    private readonly SourceList<GroundStationViewModel> _groundStation = new();
    private readonly ReadOnlyObservableCollection<GroundStationViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly GroundStationProvider? _provider;

    public GroundStationTabViewModel()
    {
        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _provider = Services.Locator.GetRequiredService<GroundStationProvider>();
        _layerStyleManager = Services.Locator.GetRequiredService<LayerStyleManager>();

        Title = "Просмотр наземных станций";
        Key = nameof(GroundStationTabViewModel);

        _groundStation
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        _layerStyleManager.Selected
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(UpdatePaletteImpl);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private void UpdatePaletteImpl((LayerType, LayerStyleViewModel?) value)
    {
        if (value.Item1 == LayerType.GroundStation
            && value.Item2?.Palette is ISingleHuePalette palette)
        {
            foreach (var item in Items)
            {
                item.Palette = palette;
            }
        }
    }

    private async Task UpdateImpl()
    {
        var ps = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            var palette = _layerStyleManager.GetPalette<ISingleHuePalette>(LayerType.GroundStation);

            var list = ps.GroundStations.Select(s => new GroundStationViewModel(s)).ToList();

            foreach (var item in list)
            {
                item.Palette = palette;
                item.UpdateObservable.Subscribe(s => _provider?.ChangedData(s.GroundStation, s.InnerAngle, s.AreaItems.Select(s => s.Angle).ToArray(), s.IsShow));
            }

            _groundStation.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }
    }

    public ReadOnlyObservableCollection<GroundStationViewModel> Items => _items;
}

public partial class GroundStationTabViewModel
{
    public GroundStationTabViewModel(DesignDataDependencyResolver resolver)
    {
        _localStorage = resolver.GetService<ILocalStorageService>();
        _provider = resolver.GetService<GroundStationProvider>();
        _layerStyleManager = resolver.GetService<LayerStyleManager>();

        Title = "Просмотр наземных станций";
        Key = nameof(GroundStationTabViewModel);
        _groundStation
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        _layerStyleManager.Selected
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(UpdatePaletteImpl);
    }
}