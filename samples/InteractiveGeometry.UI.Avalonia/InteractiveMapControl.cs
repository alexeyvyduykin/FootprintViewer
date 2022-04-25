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
            get { return GetValue(MapObserverProperty); }
            set { SetValue(MapObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly StyledProperty<IMapObserver> MapObserverProperty =
            AvaloniaProperty.Register<InteractiveMapControl, IMapObserver>(nameof(MapObserver));

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.Handled)
            {
                return;
            }

            e.Pointer.Capture(null);

            var args = e.ToMouseReleasedEventArgs(this);

            Controller.HandleMouseUp(this, args);

            //e.Handled = args.Handled;
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e.Handled == true)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseMove(this, args);

            e.Handled = args.Handled;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.Handled)
            {
                return;
            }

            Focus();
            e.Pointer.Capture(this);

            var args = e.ToMouseDownEventArgs(this);

            Controller.HandleMouseDown(this, args);

            //e.Handled = args.Handled;
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseWheelEventArgs(this);

            Controller.HandleMouseWheel(this, args);

            //e.Handled = args.Handled;
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseLeave(this, args);

            //e.Handled = args.Handled;
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);

            if (e.Handled)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);

            Controller.HandleMouseEnter(this, args);

            //e.Handled = args.Handled;
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
