using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.StateMachines;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FootprintViewer.ViewModels.ToolBar;

public sealed class CustomToolBar2ViewModel : ViewModelBase
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;
    private readonly IDataManager _dataManager;
    private readonly SourceList<MapResource> _mapResources = new();
    private readonly ReadOnlyObservableCollection<MenuItemViewModel> _mapItems;

    public CustomToolBar2ViewModel(IReadonlyDependencyResolver dependencyResolver) : base()
    {
        Tools = new ObservableCollection<IToolItem>();

        _dependencyResolver = dependencyResolver;

        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        var map = (Map)dependencyResolver.GetExistingService<IMap>();
        var mapState = dependencyResolver.GetExistingService<MapState>();

        ZoomIn = new ToolClick()
        {
            Tag = "ZoomIn",
        };

        ZoomOut = new ToolClick()
        {
            Tag = "ZoomOut",
        };

        AddRectangle = CreateToolCheck(mapState.Change, () => mapState.RectAOI(), () => mapState.IsInState(States.DrawRectangleAoI), "AddRectangle");
        AddPolygon = CreateToolCheck(mapState.Change, () => mapState.PolygonAOI(), () => mapState.IsInState(States.DrawPolygonAoI), "AddPolygon");
        AddCircle = CreateToolCheck(mapState.Change, () => mapState.CircleAOI(), () => mapState.IsInState(States.DrawCircleAoI), "AddCircle");

        AOICollection = new ToolCollection2();
        AOICollection.AddItem(AddRectangle);
        AOICollection.AddItem(AddPolygon);
        AOICollection.AddItem(AddCircle);

        RouteDistance = CreateToolCheck(mapState.Change, () => mapState.Route(), () => mapState.IsInState(States.DrawRoute), "Route");

        MapBackgrounds = new ToolCheck2()
        {
            Tag = "MapBackgrounds",
        };

        //MapLayers = new ToolCheck2()
        //{
        //    Tag = "MapLayers",
        //};

        SelectGeometry = CreateToolCheck(mapState.Change, () => mapState.Select(), () => mapState.IsInState(States.Select), "Select");
        Point = CreateToolCheck(mapState.Change, () => mapState.Point(), () => mapState.IsInState(States.DrawPoint), "Point");
        Rectangle = CreateToolCheck(mapState.Change, () => mapState.Rect(), () => mapState.IsInState(States.DrawRectangle), "Rectangle");
        Circle = CreateToolCheck(mapState.Change, () => mapState.Circle(), () => mapState.IsInState(States.DrawCircle), "Circle");
        Polygon = CreateToolCheck(mapState.Change, () => mapState.Polygon(), () => mapState.IsInState(States.DrawPolygon), "Polygon");

        GeometryCollection = new ToolCollection2();
        GeometryCollection.AddItem(Point);
        GeometryCollection.AddItem(Rectangle);
        GeometryCollection.AddItem(Circle);
        GeometryCollection.AddItem(Polygon);

        TranslateGeometry = CreateToolCheck(mapState.Change, () => mapState.Translate(), () => mapState.IsInState(States.Translate), "Translate");
        RotateGeometry = CreateToolCheck(mapState.Change, () => mapState.Rotate(), () => mapState.IsInState(States.Rotate), "Rotate");
        ScaleGeometry = CreateToolCheck(mapState.Change, () => mapState.Scale(), () => mapState.IsInState(States.Scale), "Scale");
        EditGeometry = CreateToolCheck(mapState.Change, () => mapState.Edit(), () => mapState.IsInState(States.Edit), "Edit");

        Tools.Add(ZoomIn);
        Tools.Add(ZoomOut);
        Tools.Add(AOICollection);
        Tools.Add(RouteDistance);
        Tools.Add(SelectGeometry);
        Tools.Add(GeometryCollection);
        Tools.Add(TranslateGeometry);
        Tools.Add(RotateGeometry);
        Tools.Add(ScaleGeometry);
        Tools.Add(EditGeometry);

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

        Observable.StartAsync(UpdateMapsAsync, RxApp.MainThreadScheduler).Subscribe();

        this.WhenAnyValue(s => s.IsLayerContainerOpen)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Subscribe(_ => LayerContainer = new LayerContainerViewModel(_dependencyResolver));
    }

    private static ToolCheck2 CreateToolCheck(IObservable<Unit> update, Action? selector, Func<bool>? validator, string? tag)
    {
        return new ToolCheck2(update, selector, validator) { Tag = tag };
    }

    private ICommand SetMapCommand { get; }

    [Reactive]
    public ObservableCollection<IToolItem> Tools { get; set; }

    private async Task UpdateMapsAsync()
    {
        var maps = await _dataManager.GetDataAsync<MapResource>(DbKeys.Maps.ToString());

        _mapResources.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(maps);
        });
    }

    public void Uncheck()
    {
        foreach (var item in Tools)
        {
            if (item is IToolCheck check)
            {
                check.IsCheck = false;
            }
            else if (item is IToolCollection collection)
            {
                foreach (var itemCheck in collection.Items)
                {
                    if (itemCheck is IToolCheck toolCheck)
                    {
                        toolCheck.IsCheck = false;
                    }
                }
            }
        }
    }

    public ToolClick ZoomIn { get; }

    public ToolClick ZoomOut { get; }

    public IToolCollection2 AOICollection { get; }

    public ToolCheck2 RouteDistance { get; }

    public ToolCheck2 MapBackgrounds { get; }

    //public ToolCheck2 MapLayers { get; }

    public ToolCheck2 SelectGeometry { get; }

    public IToolCollection2 GeometryCollection { get; }

    public ToolCheck2 TranslateGeometry { get; }

    public ToolCheck2 RotateGeometry { get; }

    public ToolCheck2 ScaleGeometry { get; }

    public ToolCheck2 EditGeometry { get; }

    public ToolCheck2 AddRectangle { get; }

    public ToolCheck2 AddPolygon { get; }

    public ToolCheck2 AddCircle { get; }

    public ToolCheck2 Point { get; }

    public ToolCheck2 Rectangle { get; }

    public ToolCheck2 Circle { get; }

    public ToolCheck2 Polygon { get; }

    [Reactive]
    public bool IsLayerContainerOpen { get; set; }

    public IReadOnlyList<MenuItemViewModel> MapItems => _mapItems;

    [Reactive]
    public LayerContainerViewModel? LayerContainer { get; set; }
}
