using Avalonia.Controls;
using Avalonia.Input;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Interactivity.UI.Avalonia;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;
using PlannedScheduleOnMapSample.ViewModels;
using System;

namespace PlannedScheduleOnMapSample.Views;

public class MapAdaptor : IView
{
    private MapControl _mapControl;
    private IInteractive _interactive;
    public MapAdaptor(MapControl mapControl, IInteractive interactive)
    {
        _mapControl = mapControl;
        _interactive = interactive;
    }

    public Navigator Navigator => _mapControl.Map.Navigator;

    public IInteractive Interactive => _interactive;

    public void SetCursor(CursorType cursorType)
    {

    }
}

public partial class MainWindow : Window
{
    private IInteractive _interactive;
    private string _state;
    private IController _controller;
    private MapAdaptor _mapAdaptor;
    public MainWindow()
    {
        InitializeComponent();

        var vm = MainWindowViewModel.Instance;

        _interactive = vm.Interactive!;
        _state = vm.State;

        MapControl.Map = vm.Map;

        MapControl.PointerEntered += MapControlPointerEnter;
        MapControl.PointerExited += MapControlPointerLeave;
        MapControl.PointerWheelChanged += MapControlPointerWheelChanged;
        MapControl.PointerPressed += MapControlPointerPressed;
        MapControl.PointerMoved += MapControlPointerMoved;
        MapControl.PointerReleased += MapControlPointerReleased;

        _mapAdaptor = new(MapControl, _interactive);

        _controller = InteractiveControllerFactory.GetController(_state);
        _controller.HandleMouseEnter(_mapAdaptor, new MouseEventArgs());

        vm.InteractiveObservable.Subscribe(s =>
        {
            _interactive = s;

            _mapAdaptor = new(MapControl, _interactive);

            _controller = InteractiveControllerFactory.GetController(_state);
            _controller.HandleMouseEnter(_mapAdaptor, new MouseEventArgs());
        });
    }

    protected void MapControlPointerEnter(object? sender, PointerEventArgs args)
    {
        var screenPosition = args.GetPosition(MapControl).ToMapsui();

        var mapInfo = MapControl.GetMapInfo(screenPosition);

        _controller?.HandleMouseEnter(_mapAdaptor, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected void MapControlPointerLeave(object? sender, PointerEventArgs args)
    {
        var screenPosition = args.GetPosition(MapControl).ToMapsui();

        var mapInfo = MapControl.GetMapInfo(screenPosition);

        _controller?.HandleMouseLeave(_mapAdaptor, new MouseEventArgs { MapInfo = mapInfo });
    }

    protected void MapControlPointerWheelChanged(object? sender, PointerWheelEventArgs args)
    {
        var args1 = new MouseWheelEventArgs
        {
            Delta = (int)(args.Delta.Y + args.Delta.X) * 120
        };

        _controller?.HandleMouseWheel(_mapAdaptor, args1);
    }

    protected void MapControlPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        //this.Focus();

        //e.Pointer.Capture(this);

        var screenPosition = args.GetPosition(MapControl).ToMapsui();

        var mapInfo = MapControl.GetMapInfo(screenPosition);

        var args1 = new MouseDownEventArgs
        {
#pragma warning disable CS0618 // Тип или член устарел
            ChangedButton = args.GetCurrentPoint(null).Properties.PointerUpdateKind.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
            ClickCount = args.ClickCount,
            MapInfo = mapInfo
        };

        _controller?.HandleMouseDown(_mapAdaptor, args1);
    }

    protected void MapControlPointerMoved(object? sender, PointerEventArgs args)
    {
        var screenPosition = args.GetPosition(MapControl).ToMapsui();

        // var mapInfo = MapControl.GetMapInfo(screenPosition);

        var args1 = new MouseEventArgs
        {
            //   MapInfo = mapInfo,
        };

        //  _controller?.HandleMouseMove(_mapAdaptor, args1);

        // TODO: ?
        //  e.Handled = args.Handled;
    }

    protected void MapControlPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        //e.Pointer.Capture(null);

        var screenPosition = args.GetPosition(MapControl).ToMapsui();

        var mapInfo = MapControl.GetMapInfo(screenPosition);

        _controller?.HandleMouseUp(_mapAdaptor, new MouseEventArgs { MapInfo = mapInfo });
    }
}