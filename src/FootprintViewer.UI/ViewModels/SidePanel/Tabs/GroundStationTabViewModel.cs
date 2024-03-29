﻿using DynamicData;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Models;
using FootprintViewer.Services;
using FootprintViewer.Styles;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.SidePanel.Tabs;

public sealed class GroundStationTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly LayerStyleManager _layerStyleManager;
    private readonly SourceList<GroundStationViewModel> _groundStation = new();
    private readonly ReadOnlyObservableCollection<GroundStationViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly GroundStationProvider? _layerProvider;

    public GroundStationTabViewModel()
    {
        Title = "Ground stations viewer";

        Key = nameof(GroundStationTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var mapService = Services.Locator.GetRequiredService<IMapService>();
        _layerProvider = mapService.GetProvider<GroundStationProvider>();
        _layerStyleManager = mapService.LayerStyle;

        var mainObservable = _groundStation
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
            .Transform(s => s.GroundStation)
            .ToCollection();

        _layerProvider?.SetObservable(layerObservable);

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
                item.UpdateObservable
                    .Subscribe(s => _layerProvider?.ChangedData(s.GroundStation, s.InnerAngle, s.AreaItems.Select(s => s.Angle).ToArray(), s.IsShow));
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