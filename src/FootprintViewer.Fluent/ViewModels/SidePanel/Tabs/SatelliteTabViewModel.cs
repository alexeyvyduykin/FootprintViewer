using DynamicData;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Fluent.Services2;
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

public sealed class SatelliteTabViewModel : SidePanelTabViewModel
{
    private readonly TrackProvider? _trackProvider;
    private readonly SensorProvider? _sensorProvider;
    private readonly SourceList<SatelliteViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ILocalStorageService _localStorage;
    private readonly LayerStyleManager _layerStyleManager;

    public SatelliteTabViewModel()
    {
        Title = "Satellites viewer";

        Key = nameof(SatelliteTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var mapService = Services.Locator.GetRequiredService<IMapService>();

        _trackProvider = mapService.GetProvider<TrackProvider>();
        _sensorProvider = mapService.GetProvider<SensorProvider>();
        _layerStyleManager = mapService.LayerStyle;

        var mainObservable = _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler);

        mainObservable
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _localStorage.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        var layerObservable = mainObservable
            .Transform(s => s.Satellite)
            .ToCollection();

        _trackProvider?.SetObservable(layerObservable);
        _sensorProvider?.SetObservable(layerObservable);

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
            .Subscribe(UpdateColorImpl);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private void UpdateColorImpl((LayerType, LayerStyleViewModel?) value)
    {
        if ((value.Item1 == LayerType.Track || value.Item1 == LayerType.Sensor)
            && value.Item2?.Palette is IColorPalette palette)
        {
            foreach (var item in Items)
            {
                item.Color = palette.PickColor(item.Name).ToMapsuiColor();
            }
        }
    }

    private async Task UpdateImpl()
    {
        var ps = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            var palette = _layerStyleManager.GetPalette<IColorPalette>(LayerType.Track);

            var list = ps.Satellites.Select(s => new SatelliteViewModel(s)).ToList();

            foreach (var item in list)
            {
                item.TrackObservable
                    .Subscribe(s => _trackProvider?.ChangedData(s.Satellite, s.CurrentNode - 1, s.IsShow));
                item.SwathsObservable
                    .Subscribe(s => _sensorProvider?.ChangedData(s.Satellite, s.CurrentNode - 1, s.IsShow && s.IsLeftSwath, s.IsShow && s.IsRightSwath));
                item.Color = palette?.PickColor(item.Name).ToMapsuiColor();
            }

            _satellites.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(list);
            });
        }
    }

    public ReadOnlyObservableCollection<SatelliteViewModel> Items => _items;
}