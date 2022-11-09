using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class FootprintPreviewTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly Map _map;
    private readonly IMapNavigator _mapNavigator;
    private readonly SourceList<FootprintPreview> _footprintPreviews = new();
    private readonly SourceList<FootprintPreviewGeometry> _geometries = new();
    private readonly ReadOnlyObservableCollection<FootprintPreviewViewModel> _items;
    private readonly ReadOnlyObservableCollection<FootprintPreviewGeometry> _geometryItems;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;

    public FootprintPreviewTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _map = (Map)dependencyResolver.GetExistingService<IMap>();
        _mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        Filter = new FootprintPreviewTabFilterViewModel(dependencyResolver);

        Title = "Поиск сцены";

        _footprintPreviews
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new FootprintPreviewViewModel(s))
            .Filter(Filter.FilterObservable)
            .Bind(out _items)
            .Subscribe();

        _geometries
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _geometryItems)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.IsLoading);

        Enter = ReactiveCommand.Create<FootprintPreviewViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        this.WhenAnyValue(s => s.SelectedItem)
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(ReactiveCommand.Create<FootprintPreviewViewModel?>(SelectImpl));

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .ToSignal()
            .InvokeCommand(Update);
    }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<FootprintPreviewViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        var footprintPreviews = await _dataManager.GetDataAsync<FootprintPreview>(DbKeys.FootprintPreviews.ToString());
        var geometries = await _dataManager.GetDataAsync<FootprintPreviewGeometry>(DbKeys.FootprintPreviewGeometries.ToString());

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
            var layer = _map.GetLayer(LayerType.FootprintImageBorder);

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
        var layer = _map.GetLayer(LayerType.FootprintImageBorder);

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

            _map.ReplaceLayer(layer, LayerType.FootprintImage);

            var geometry = Geometries
                .Where(s => Equals(s.Name, footprintPreview.Name))
                .Select(s => s.Geometry)
                .FirstOrDefault();

            if (geometry != null)
            {
                _mapNavigator.SetFocusToPoint(geometry.Centroid.ToMPoint());
            }
        }
    }

    public void SetAOI(Geometry aoi) => ((FootprintPreviewTabFilterViewModel)Filter).AOI = aoi;

    public void ResetAOI() => ((FootprintPreviewTabFilterViewModel)Filter).AOI = null;

    public IFilter<FootprintPreviewViewModel> Filter { get; }

    private ReadOnlyObservableCollection<FootprintPreviewGeometry> Geometries => _geometryItems;

    public ReadOnlyObservableCollection<FootprintPreviewViewModel> Items => _items;

    [Reactive]
    public FootprintPreviewViewModel? SelectedItem { get; set; }
}
