using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class SatelliteTabViewModel : SidePanelTabViewModel
{
    private readonly TrackProvider? _trackProvider;
    private readonly SensorProvider? _sensorProvider;
    private readonly SourceList<SatelliteViewModel> _satellites = new();
    private readonly ReadOnlyObservableCollection<SatelliteViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly IDataManager _dataManager;
    private readonly LayerStyleManager _layerStyleManager;

    public SatelliteTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        _trackProvider = dependencyResolver.GetExistingService<TrackProvider>();
        _sensorProvider = dependencyResolver.GetExistingService<SensorProvider>();
        _layerStyleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        Title = "Просмотр спутников";

        _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Satellites.ToString()))
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
        var palette = _layerStyleManager.GetPalette<IColorPalette>(LayerType.Track);
        var res = await _dataManager.GetDataAsync<Satellite>(DbKeys.Satellites.ToString());

        var list = res.Select(s => new SatelliteViewModel(s)).ToList();

        foreach (var item in list)
        {
            item.TrackObservable.Subscribe(s => _trackProvider?.ChangedData(s));
            item.SwathsObservable.Subscribe(s => _sensorProvider?.ChangedData(s));
            item.Color = palette?.PickColor(item.Name).ToMapsuiColor();
        }

        _satellites.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    public ReadOnlyObservableCollection<SatelliteViewModel> Items => _items;
}
