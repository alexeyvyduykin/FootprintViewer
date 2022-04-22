using Avalonia;
using Avalonia.Input;
using InteractiveGeometry.UI.Input;
using Mapsui;
using Mapsui.UI.Avalonia;
using System;

namespace InteractiveGeometry.UI.Avalonia
{
    public class InteractiveMapControl : MapControl, IMapView
    {
        public InteractiveMapControl() : base()
        {
            PointerEnter += MyMapControl_MouseEnter;
            PointerLeave += MyMapControl_MouseLeave;
            PointerWheelChanged += MyMapControl_MouseWheel;
            PointerPressed += MyMapControl_MouseDown;
            PointerMoved += MyMapControl_MouseMove;
            PointerReleased += MyMapControl_MouseUp;

            ControllerProperty.Changed.Subscribe(OnControllerChanged);
        }

        public IController Controller
        {
            get { return GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IController> ControllerProperty =
            AvaloniaProperty.Register<InteractiveMapControl, IController>(nameof(Controller), new DefaultController());

        private static void OnControllerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var mapControl = (InteractiveMapControl)e.Sender;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new Input.Core.MouseEventArgs());
        }

        public IMapObserver MapObserver
        {
            get { return (IMapObserver)GetValue(MapObserverProperty); }
            set { SetValue(MapObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IMapObserver> MapObserverProperty =
            AvaloniaProperty.Register<InteractiveMapControl, IMapObserver>(nameof(MapObserver));

        private void MyMapControl_MouseUp(object? sender, PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            e.Pointer.Capture(null);

            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));
        }

        private void MyMapControl_MouseMove(object? sender, PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            Controller.HandleMouseMove(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseDown(object? sender, PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            Focus();

            e.Pointer.Capture(this);

            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object? sender, PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object? sender, PointerEventArgs e)
        {
            base.OnPointerLeave(e);

            Controller.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object? sender, PointerEventArgs e)
        {
            base.OnPointerEnter(e);

            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public void SetCursor(CursorType cursorType)
        {
            Cursor = cursorType switch
            {
                CursorType.Default => new Cursor(StandardCursorType.Arrow),
                CursorType.Hand => new Cursor(StandardCursorType.Hand),
                CursorType.HandGrab => new Cursor(StandardCursorType.SizeAll),
                CursorType.Cross => new Cursor(StandardCursorType.Cross),
                _ => throw new Exception(),
            };
        }

        public MPoint ScreenToWorld(MPoint screenPosition) => Viewport.ScreenToWorld(screenPosition);

        public MPoint WorldToScreen(MPoint worldPosition) => Viewport.WorldToScreen(worldPosition);
    }
}
