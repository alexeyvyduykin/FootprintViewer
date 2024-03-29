﻿using DynamicData;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Models;
using FootprintViewer.Services;
using FootprintViewer.UI.Extensions;
using FootprintViewer.UI.Services2;
using FootprintViewer.UI.ViewModels.SidePanel.Filters;
using FootprintViewer.UI.ViewModels.SidePanel.Items;
using Mapsui.Layers;
using Mapsui.Nts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.SidePanel.Tabs;

public sealed class FootprintPreviewTabViewModel : SidePanelTabViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly IMapService _mapService;
    private readonly SourceList<FootprintPreview> _footprintPreviews = new();
    private readonly SourceList<FootprintPreviewGeometry> _geometries = new();
    private readonly ReadOnlyObservableCollection<FootprintPreviewViewModel> _items;
    private readonly ReadOnlyObservableCollection<FootprintPreviewGeometry> _geometryItems;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public FootprintPreviewTabViewModel()
    {
        Title = "Scene search";
        Key = nameof(FootprintPreviewTabViewModel);

        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();
        _mapService = Services.Locator.GetRequiredService<IMapService>();

        Filter = new FootprintPreviewTabFilterViewModel();
        Filter.SetAOIObservable(_mapService.AOI.Changed);

        var filter1 = Filter.AOIObservable;
        var filter2 = Filter.FilterObservable;
        var filter3 = this.WhenAnyValue(s => s.SearchString)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(SearchStringPredicate);

        _footprintPreviews
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => ItemCount = _footprintPreviews.Count);

        _footprintPreviews
            .Connect()
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Transform(s => new FootprintPreviewViewModel(s))
            .Filter(filter1)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(filter2)
            .Filter(filter3)
            .Bind(out _items)
            .Subscribe(_ => FilteringItemCount = _items.Count);

        this.WhenAnyValue(s => s.FilteringItemCount)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => IsFilteringActive = s != ItemCount);

        _geometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _geometryItems)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        _localStorage.DataChanged
            .Where(s => new[] { DbKeys.FootprintPreviews.ToString(), DbKeys.FootprintPreviewGeometries.ToString() }.Any(key => s.Contains(key)))
            .ToSignal()
            .InvokeCommand(Update);

        Enter = ReactiveCommand.Create<FootprintPreviewViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        EmptySearchString = ReactiveCommand.Create(() => { SearchString = string.Empty; }, outputScheduler: RxApp.MainThreadScheduler);

        this.WhenAnyValue(s => s.SelectedItem)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(ReactiveCommand.Create<FootprintPreviewViewModel?>(SelectImpl));

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<FootprintPreviewViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var footprintPreviews = await _localStorage.GetValuesAsync<FootprintPreview>(DbKeys.FootprintPreviews.ToString());
        var geometries = await _localStorage.GetValuesAsync<FootprintPreviewGeometry>(DbKeys.FootprintPreviewGeometries.ToString());

        _footprintPreviews.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(footprintPreviews);
        });

        _geometries.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(geometries);
        });
    }

    private void EnterImpl(FootprintPreviewViewModel footprintPreview)
    {
        var geometry = Geometries
            .Where(s => Equals(s.Name, footprintPreview.Name))
            .Select(s => s.Geometry)
            .FirstOrDefault();

        if (geometry != null)
        {
            var layer = _mapService.Map.GetLayer(LayerType.FootprintImageBorder);

            if (layer != null && layer is WritableLayer writableLayer)
            {
                writableLayer.Clear();
                writableLayer.Add(new GeometryFeature() { Geometry = geometry });
                writableLayer.DataHasChanged();
            }
        }
    }

    private void LeaveImpl()
    {
        var layer = _mapService.Map.GetLayer(LayerType.FootprintImageBorder);

        if (layer != null && layer is WritableLayer writableLayer)
        {
            writableLayer.Clear();
            writableLayer.DataHasChanged();
        }
    }

    private void SelectImpl(FootprintPreviewViewModel? footprintPreview)
    {
        if (footprintPreview != null && footprintPreview.Path != null)
        {
            var layer = MapsuiHelper.CreateMbTilesLayer(footprintPreview.Path);

            _mapService.Map.ReplaceLayer(layer, LayerType.FootprintImage);

            var geometry = Geometries
                .Where(s => Equals(s.Name, footprintPreview.Name))
                .Select(s => s.Geometry)
                .FirstOrDefault();

            _mapService.FlyToFootprintPreview(geometry);
        }
    }

    private static Func<FootprintPreviewViewModel, bool> SearchStringPredicate(string? arg)
    {
        return (s =>
        {
            if (string.IsNullOrEmpty(arg) == true)
            {
                return true;
            }

            return s.TileNumber?.Contains(arg, StringComparison.CurrentCultureIgnoreCase) ?? true;
        });
    }

    public IAOIFilter<FootprintPreviewViewModel> Filter { get; }

    private ReadOnlyObservableCollection<FootprintPreviewGeometry> Geometries => _geometryItems;

    public ReadOnlyObservableCollection<FootprintPreviewViewModel> Items => _items;

    [Reactive]
    public string? SearchString { get; set; }

    public ReactiveCommand<Unit, Unit> EmptySearchString { get; }

    [Reactive]
    public FootprintPreviewViewModel? SelectedItem { get; set; }

    [Reactive]
    public int ItemCount { get; set; }

    [Reactive]
    public int FilteringItemCount { get; set; }

    [Reactive]
    public bool IsFilteringActive { get; set; }
}
