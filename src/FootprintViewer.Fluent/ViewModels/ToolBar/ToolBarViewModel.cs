using DynamicData;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Extensions;
using FootprintViewer.Fluent.Services2;
using FootprintViewer.Services;
using FootprintViewer.StateMachines;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public sealed class ToolBarViewModel : ViewModelBase
{
    private readonly ILocalStorageService _storage;
    private readonly SourceList<MapResource> _mapResources = new();
    private readonly ReadOnlyObservableCollection<MenuItemViewModel> _mapItems;

    public ToolBarViewModel() : base()
    {
        _storage = Services.Locator.GetRequiredService<ILocalStorageService>();
        var mapService = Services.Locator.GetRequiredService<IMapService>();
        var state = mapService.State;

        ZoomIn = new ToolClick()
        {
            Tag = "ZoomIn",
        };

        ZoomOut = new ToolClick()
        {
            Tag = "ZoomOut",
        };

        ZoomIn.SubscribeAsync(mapService.ZoomIn);
        ZoomOut.SubscribeAsync(mapService.ZoomOut);

        AddRectangle = CreateToolCheck(state.Callback, state.RectAOI, () => state.IsInState(States.DrawRectangleAoI), "AddRectangle", "AddRectangle");
        AddPolygon = CreateToolCheck(state.Callback, state.PolygonAOI, () => state.IsInState(States.DrawPolygonAoI), "AddPolygon", "AddPolygon");
        AddCircle = CreateToolCheck(state.Callback, state.CircleAOI, () => state.IsInState(States.DrawCircleAoI), "AddCircle", "AddCircle");

        AOICollection = CreateToolCollection(new[] { AddRectangle, AddPolygon, AddCircle });

        RouteDistance = CreateToolCheck(state.Callback, state.Route, () => state.IsInState(States.DrawRoute), "Route", "Route");

        MapBackgrounds = new ToolCheck()
        {
            Tag = "MapBackgrounds",
        };

        MapLayers = new ToolCheck()
        {
            Tag = "MapLayers",
        };

        SelectGeometry = CreateToolCheck(state.Callback, state.Select, () => state.IsInState(States.Select), "Select", "Select");
        Point = CreateToolCheck(state.Callback, state.Point, () => state.IsInState(States.DrawPoint), "AddPoint", "Point");
        Rectangle = CreateToolCheck(state.Callback, state.Rect, () => state.IsInState(States.DrawRectangle), "AddRectangle", "Rectangle");
        Circle = CreateToolCheck(state.Callback, state.Circle, () => state.IsInState(States.DrawCircle), "AddCircle", "Circle");
        Polygon = CreateToolCheck(state.Callback, state.Polygon, () => state.IsInState(States.DrawPolygon), "AddPolygon", "Polygon");

        GeometryCollection = CreateToolCollection(new[] { Point, Rectangle, Circle, Polygon });

        TranslateGeometry = CreateToolCheck(state.Callback, state.Translate, () => state.IsInState(States.Translate), "Translate", "Translate");
        RotateGeometry = CreateToolCheck(state.Callback, state.Rotate, () => state.IsInState(States.Rotate), "Rotate", "Rotate");
        ScaleGeometry = CreateToolCheck(state.Callback, state.Scale, () => state.IsInState(States.Scale), "Scale", "Scale");
        EditGeometry = CreateToolCheck(state.Callback, state.Edit, () => state.IsInState(States.Edit), "Edit", "Edit");

        SetMapCommand = ReactiveCommand.Create<MapResource>(mapService.Map.SetWorldMapLayer);

        _mapResources
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new MenuItemViewModel()
            {
                Header = s.Name,
                Command = SetMapCommand,
                CommandParameter = s,
            })
            .Bind(out _mapItems)
            .Subscribe();

        _storage.DataChanged
            .Where(s => s.Contains(DbKeys.Maps.ToString()))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateMapsAsync));

        Observable.StartAsync(UpdateMapsAsync, RxApp.MainThreadScheduler).Subscribe();

        this.WhenAnyValue(s => s.IsLayerContainerOpen)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Subscribe(_ => LayerContainer = new LayerContainerViewModel());
    }

    private static ToolCheck CreateToolCheck(IObservable<Unit> callback, Action? selector, Func<bool>? validator, string? key, string? tag)
    {
        return new ToolCheck(callback, selector, validator)
        {
            Key = key,
            Tag = tag
        };
    }

    private static IToolCollection CreateToolCollection(IToolCheck[] toolChecks)
    {
        var collection = new ToolCollection();

        foreach (var item in toolChecks)
        {
            collection.AddItem(item);
        }

        return collection;
    }

    private async Task UpdateMapsAsync()
    {
        var maps = await _storage.GetValuesAsync<MapResource>(DbKeys.Maps.ToString());

        _mapResources.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(maps);
        });
    }

    private ICommand SetMapCommand { get; }

    public ToolClick ZoomIn { get; }

    public ToolClick ZoomOut { get; }

    public IToolCollection AOICollection { get; }

    public ToolCheck RouteDistance { get; }

    public ToolCheck MapBackgrounds { get; }

    public ToolCheck MapLayers { get; }

    public ToolCheck SelectGeometry { get; }

    public IToolCollection GeometryCollection { get; }

    public ToolCheck TranslateGeometry { get; }

    public ToolCheck RotateGeometry { get; }

    public ToolCheck ScaleGeometry { get; }

    public ToolCheck EditGeometry { get; }

    public ToolCheck AddRectangle { get; }

    public ToolCheck AddPolygon { get; }

    public ToolCheck AddCircle { get; }

    public ToolCheck Point { get; }

    public ToolCheck Rectangle { get; }

    public ToolCheck Circle { get; }

    public ToolCheck Polygon { get; }

    [Reactive]
    public bool IsLayerContainerOpen { get; set; }

    public IReadOnlyList<MenuItemViewModel> MapItems => _mapItems;

    [Reactive]
    public LayerContainerViewModel? LayerContainer { get; set; }
}