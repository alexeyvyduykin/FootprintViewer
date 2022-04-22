using Avalonia;
using Avalonia.Input;
using InteractiveGeometry;
using InteractiveGeometry.UI;
using InteractiveGeometry.UI.Input;
using InteractiveSample.ViewModels;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.UI;
using Mapsui.UI.Avalonia;
using System;
using System.Reactive.Linq;
using input = InteractiveGeometry.UI;

namespace InteractiveSample
{
    public class UserMapControl : MapControl, IMapView
    {
        private bool _isLeftMouseDown = false;
        private bool _isLeftClick = false;
        private MapInfo? _lastMapInfo;

        private int _counter = 0;

        public UserMapControl() : base()
        {
            PointerEnter += MyMapControl_MouseEnter;
            PointerLeave += MyMapControl_MouseLeave;
            PointerWheelChanged += MyMapControl_MouseWheel;
            PointerPressed += MyMapControl_MouseDown;
            PointerMoved += MyMapControl_MouseMove;
            PointerReleased += MyMapControl_MouseUp;

            MapListenerProperty.Changed.Subscribe(OnMapListenerChanged);
            MapSourceProperty.Changed.Subscribe(OnMapSourceChanged);
            ControllerProperty.Changed.Subscribe(OnControllerChanged);
        }

        private static void UserMapControl_Info(object? sender, MapInfoEventArgs e)
        {
            var s = (UserMapControl?)sender;

            if (s != null)
            {
                if (e.MapInfo != null && e.MapInfo.Feature != null)
                {
                    s.SetMapInfo(e.MapInfo);
                }
            }
        }

        private void SetMapInfo(MapInfo mapInfo)
        {
            _lastMapInfo = mapInfo;
            _isLeftClick = true;
        }

        public MapListener? MapListener
        {
            get { return (MapListener?)GetValue(MapListenerProperty); }
            set { SetValue(MapListenerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<MapListener?> MapListenerProperty =
            AvaloniaProperty.Register<UserMapControl, MapListener?>(nameof(MapListener));

        private static void OnMapListenerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var s = (UserMapControl)e.Sender;

            if (e.NewValue != null && e.NewValue is MapListener mapListener)
            {
                if (s.MapListener != null)
                {
                    s.Info -= UserMapControl_Info;
                }

                s.Info += UserMapControl_Info;
            }
        }

        public Map MapSource
        {
            get { return (Map)GetValue(MapSourceProperty); }
            set { SetValue(MapSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapSource.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<Map> MapSourceProperty =
            AvaloniaProperty.Register<UserMapControl, Map>(nameof(MapSource));

        private static void OnMapSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is Map map)
                {
                    mapControl.Map = map;

                    mapControl.Navigator.CenterOn(SphericalMercator.FromLonLat(13, 42).ToMPoint());
                    mapControl.Navigator.ZoomTo(1000);
                }
            }
        }

        public IController Controller
        {
            get { return GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IController> ControllerProperty =
            AvaloniaProperty.Register<UserMapControl, IController>(nameof(Controller), new DefaultController());

        private static void OnControllerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)e.Sender;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new input.Input.Core.MouseEventArgs());
        }

        public IMapObserver MapObserver
        {
            get { return (IMapObserver)GetValue(MapObserverProperty); }
            set { SetValue(MapObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IMapObserver> MapObserverProperty =
            AvaloniaProperty.Register<UserMapControl, IMapObserver>(nameof(MapObserver));

        public void SetCursor(input.CursorType cursorType, string info = "")
        {
            switch (cursorType)
            {
                case input.CursorType.Default:
                    Cursor = new Cursor(StandardCursorType.Arrow);
                    break;
                case input.CursorType.Hand:
                    Cursor = new Cursor(StandardCursorType.Hand);
                    break;
                case input.CursorType.HandGrab:
                    Cursor = new Cursor(StandardCursorType.SizeAll);
                    break;
                case input.CursorType.Cross:
                    Cursor = new Cursor(StandardCursorType.Cross);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void MyMapControl_MouseUp(object? sender, PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (e.Handled)
            {
                return;
            }

            e.Pointer.Capture(null);

            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

            if (_isLeftClick == true)
            {
                MapListener?.LeftClick(_lastMapInfo);

                _isLeftClick = false;
            }

            _isLeftMouseDown = false;
        }

        private void MyMapControl_MouseMove(object? sender, PointerEventArgs e)
        {
            if (++_counter > 1)
            {
                _isLeftClick = false;
            }

            base.OnPointerMoved(e);

            if (e.Handled == true)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);
            Controller.HandleMouseMove(this, args);

            if (args.Handled == false)
            {
                if (_isLeftMouseDown == true && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
                {
                    SetCursor(input.CursorType.HandGrab);
                }
                else
                {
                    SetCursor(input.CursorType.Default);
                }
            }
        }

        private void MyMapControl_MouseDown(object? sender, PointerPressedEventArgs e)
        {
            _counter = 0;

            base.OnPointerPressed(e);
            if (e.Handled)
            {
                return;
            }

            Focus();
            e.Pointer.Capture(this);

            // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
            //  _mouseDownPoint = e.GetPosition(this).ToScreenPoint();

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
            {
                _isLeftMouseDown = true;
            }

            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object? sender, PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object? sender, PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            Controller.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object? sender, PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public MPoint ScreenToWorld(MPoint screenPosition)
        {
            return Viewport.ScreenToWorld(screenPosition);
        }

        public MPoint WorldToScreen(MPoint worldPosition)
        {
            return Viewport.WorldToScreen(worldPosition);
        }
    }
}
