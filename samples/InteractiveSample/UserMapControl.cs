using Avalonia;
using Avalonia.Input;
using InteractiveGeometry;
using InteractiveGeometry.UI;
using InteractiveGeometry.UI.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.UI.Avalonia;
using System;
using System.Reactive.Linq;
using input = InteractiveGeometry.UI;

namespace InteractiveSample
{
    public class UserMapControl : MapControl, IMapView
    {
        private bool _isLeftMouseDown = false;

        public UserMapControl() : base()
        {
            PointerEnter += MyMapControl_MouseEnter;
            PointerLeave += MyMapControl_MouseLeave;
            PointerWheelChanged += MyMapControl_MouseWheel;
            PointerPressed += MyMapControl_MouseDown;
            PointerMoved += MyMapControl_MouseMove;
            PointerReleased += MyMapControl_MouseUp;

            MapSourceProperty.Changed.Subscribe(OnMapSourceChanged);
            ControllerProperty.Changed.Subscribe(OnControllerChanged);
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

        public void SetCursor(input.CursorType cursorType)
        {
            Cursor = cursorType switch
            {
                input.CursorType.Default => new Cursor(StandardCursorType.Arrow),
                input.CursorType.Hand => new Cursor(StandardCursorType.Hand),
                input.CursorType.HandGrab => new Cursor(StandardCursorType.SizeAll),
                input.CursorType.Cross => new Cursor(StandardCursorType.Cross),
                _ => throw new Exception(),
            };
        }

        //protected override void OnPointerReleased(PointerReleasedEventArgs e)        
        private void MyMapControl_MouseUp(object? sender, PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.Handled)
            {
                return;
            }

            e.Pointer.Capture(null);

            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

            _isLeftMouseDown = false;
        }

        //protected override void OnPointerMoved(PointerEventArgs e)
        private void MyMapControl_MouseMove(object? sender, PointerEventArgs e)
        {
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

        //protected override void OnPointerPressed(PointerPressedEventArgs e)
        private void MyMapControl_MouseDown(object? sender, PointerPressedEventArgs e)
        {
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

        //protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        private void MyMapControl_MouseWheel(object? sender, PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        //protected override void OnPointerLeave(PointerEventArgs e)
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

        //protected override void OnPointerEnter(PointerEventArgs e)
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
