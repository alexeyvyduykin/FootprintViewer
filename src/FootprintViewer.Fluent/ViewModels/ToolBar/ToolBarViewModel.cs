using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Designer;
using FootprintViewer.StateMachines;
using Mapsui;
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

public sealed partial class ToolBarViewModel : ViewModelBase
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<MapResource> _mapResources = new();
    private readonly ReadOnlyObservableCollection<MenuItemViewModel> _mapItems;

    public ToolBarViewModel() : base()
    {
        _dataManager = Services.DataManager;
        var map = Services.Map;
        var mapState = Services.MapState;

        ZoomIn = new ToolClick()
        {
            Tag = "ZoomIn",
        };

        ZoomOut = new ToolClick()
        {
            Tag = "ZoomOut",
        };

        AddRectangle = CreateToolCheck(mapState.Change, () => mapState.RectAOI(), () => mapState.IsInState(States.DrawRectangleAoI), "AddRectangle", "AddRectangle");
        AddPolygon = CreateToolCheck(mapState.Change, () => mapState.PolygonAOI(), () => mapState.IsInState(States.DrawPolygonAoI), "AddPolygon", "AddPolygon");
        AddCircle = CreateToolCheck(mapState.Change, () => mapState.CircleAOI(), () => mapState.IsInState(States.DrawCircleAoI), "AddCircle", "AddCircle");

        AOICollection = CreateToolCollection(new[] { AddRectangle, AddPolygon, AddCircle });

        RouteDistance = CreateToolCheck(mapState.Change, () => mapState.Route(), () => mapState.IsInState(States.DrawRoute), "Route", "Route");

        MapBackgrounds = new ToolCheck()
        {
            Tag = "MapBackgrounds",
        };

        MapLayers = new ToolCheck()
        {
            Tag = "MapLayers",
        };

        SelectGeometry = CreateToolCheck(mapState.Change, () => mapState.Select(), () => mapState.IsInState(States.Select), "Select", "Select");
        Point = CreateToolCheck(mapState.Change, () => mapState.Point(), () => mapState.IsInState(States.DrawPoint), "AddPoint", "Point");
        Rectangle = CreateToolCheck(mapState.Change, () => mapState.Rect(), () => mapState.IsInState(States.DrawRectangle), "AddRectangle", "Rectangle");
        Circle = CreateToolCheck(mapState.Change, () => mapState.Circle(), () => mapState.IsInState(States.DrawCircle), "AddCircle", "Circle");
        Polygon = CreateToolCheck(mapState.Change, () => mapState.Polygon(), () => mapState.IsInState(States.DrawPolygon), "AddPolygon", "Polygon");

        GeometryCollection = CreateToolCollection(new[] { Point, Rectangle, Circle, Polygon });

        TranslateGeometry = CreateToolCheck(mapState.Change, () => mapState.Translate(), () => mapState.IsInState(States.Translate), "Translate", "Translate");
        RotateGeometry = CreateToolCheck(mapState.Change, () => mapState.Rotate(), () => mapState.IsInState(States.Rotate), "Rotate", "Rotate");
        ScaleGeometry = CreateToolCheck(mapState.Change, () => mapState.Scale(), () => mapState.IsInState(States.Scale), "Scale", "Scale");
        EditGeometry = CreateToolCheck(mapState.Change, () => mapState.Edit(), () => mapState.IsInState(States.Edit), "Edit", "Edit");

        SetMapCommand = ReactiveCommand.Create<MapResource>(s => map.SetWorldMapLayer(s));

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

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Maps.ToString()))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateMapsAsync));

        Observable.StartAsync(UpdateMapsAsync, RxApp.MainThreadScheduler).Subscribe();

        this.WhenAnyValue(s => s.IsLayerContainerOpen)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Subscribe(_ => LayerContainer = new LayerContainerViewModel());
    }

    private static ToolCheck CreateToolCheck(IObservable<Unit> update, Action? selector, Func<bool>? validator, string? key, string? tag)
    {
        return new ToolCheck(update, selector, validator) 
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
        var maps = await _dataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());

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

public partial class ToolBarViewModel
{
    public ToolBarViewModel(DesignDataDependencyResolver resolver)
    {
        _dataManager = resolver.GetService<IDataManager>();
        var map = (Map)resolver.GetService<IMap>();
        var mapState = resolver.GetService<MapState>();

        ZoomIn = new ToolClick()
        {
            Tag = "ZoomIn",
        };

        ZoomOut = new ToolClick()
        {
            Tag = "ZoomOut",
        };

        AddRectangle = CreateToolCheck(mapState.Change, () => mapState.RectAOI(), () => mapState.IsInState(States.DrawRectangleAoI), "AddRectangle", "AddRectangle");
        AddPolygon = CreateToolCheck(mapState.Change, () => mapState.PolygonAOI(), () => mapState.IsInState(States.DrawPolygonAoI), "AddPolygon", "AddPolygon");
        AddCircle = CreateToolCheck(mapState.Change, () => mapState.CircleAOI(), () => mapState.IsInState(States.DrawCircleAoI), "AddCircle", "AddCircle");

        AOICollection = CreateToolCollection(new[] { AddRectangle, AddPolygon, AddCircle });

        RouteDistance = CreateToolCheck(mapState.Change, () => mapState.Route(), () => mapState.IsInState(States.DrawRoute), "Route", "Route");

        MapBackgrounds = new ToolCheck()
        {
            Tag = "MapBackgrounds",
        };

        MapLayers = new ToolCheck()
        {
            Tag = "MapLayers",
        };

        SelectGeometry = CreateToolCheck(mapState.Change, () => mapState.Select(), () => mapState.IsInState(States.Select), "Select", "Select");
        Point = CreateToolCheck(mapState.Change, () => mapState.Point(), () => mapState.IsInState(States.DrawPoint), "AddPoint", "Point");
        Rectangle = CreateToolCheck(mapState.Change, () => mapState.Rect(), () => mapState.IsInState(States.DrawRectangle), "AddRectangle", "Rectangle");
        Circle = CreateToolCheck(mapState.Change, () => mapState.Circle(), () => mapState.IsInState(States.DrawCircle), "AddCircle", "Circle");
        Polygon = CreateToolCheck(mapState.Change, () => mapState.Polygon(), () => mapState.IsInState(States.DrawPolygon), "AddPolygon", "Polygon");

        GeometryCollection = CreateToolCollection(new[] { Point, Rectangle, Circle, Polygon });

        TranslateGeometry = CreateToolCheck(mapState.Change, () => mapState.Translate(), () => mapState.IsInState(States.Translate), "Translate", "Translate");
        RotateGeometry = CreateToolCheck(mapState.Change, () => mapState.Rotate(), () => mapState.IsInState(States.Rotate), "Rotate", "Rotate");
        ScaleGeometry = CreateToolCheck(mapState.Change, () => mapState.Scale(), () => mapState.IsInState(States.Scale), "Scale", "Scale");
        EditGeometry = CreateToolCheck(mapState.Change, () => mapState.Edit(), () => mapState.IsInState(States.Edit), "Edit", "Edit");

        SetMapCommand = ReactiveCommand.Create<MapResource>(s => map.SetWorldMapLayer(s));

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

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Maps.ToString()))
            .ToSignal()
            .InvokeCommand(ReactiveCommand.CreateFromTask(UpdateMapsAsync));

        Observable.StartAsync(UpdateMapsAsync, RxApp.MainThreadScheduler).Subscribe();

        this.WhenAnyValue(s => s.IsLayerContainerOpen)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Subscribe(_ => LayerContainer = new LayerContainerViewModel());
    }

}