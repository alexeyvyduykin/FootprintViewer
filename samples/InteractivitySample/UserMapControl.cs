using InteractivitySample.Input.Controller;
using input = InteractivitySample.Input;
using InteractivitySample.ViewModels;
using Mapsui;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using InteractivitySample.Interactivity;

namespace InteractivitySample
{
    public class UserMapControl : MapControl, IMapView
    {
        private bool _isLeftMouseDown = false;
        private bool _isLeftClick = false;
        private MapInfo? _lastMapInfo;

        private int _counter = 0;

        public UserMapControl() : base()
        {
            MouseEnter += MyMapControl_MouseEnter;
            MouseLeave += MyMapControl_MouseLeave;
            MouseWheel += MyMapControl_MouseWheel;
            MouseDown += MyMapControl_MouseDown;
            MouseMove += MyMapControl_MouseMove;
            MouseUp += MyMapControl_MouseUp;
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
        public static readonly DependencyProperty MapListenerProperty =
            DependencyProperty.Register("MapListener", typeof(MapListener), typeof(UserMapControl), new PropertyMetadata(null, OnMapListenerChanged));

        private static void OnMapListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = (UserMapControl)d;

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
        public static readonly DependencyProperty MapSourceProperty =
            DependencyProperty.Register("MapSource", typeof(Map), typeof(UserMapControl), new PropertyMetadata(null, OnMapSourceChanged));

        private static void OnMapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UserMapControl mapControl)
            {
                if (e.NewValue != null && e.NewValue is Map map)
                {
                    mapControl.Map = map;
                }
            }
        }

        public IController Controller
        {
            get { return (IController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IController), typeof(UserMapControl), new PropertyMetadata(new EditController(), OnControllerChanged));

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)d;

            // HACK: after tools check, hover manipulator not active, it call this
            mapControl.Controller.HandleMouseEnter(mapControl, new input.Controller.Core.MouseEventArgs());
        }

        public IMapObserver MapObserver
        {
            get { return (IMapObserver)GetValue(MapObserverProperty); }
            set { SetValue(MapObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Observer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapObserverProperty =
            DependencyProperty.Register("MapObserver", typeof(IMapObserver), typeof(UserMapControl));

        public void SetCursor(input.CursorType cursorType, string info = "")
        {
            switch (cursorType)
            {
                case input.CursorType.Default:
                    Cursor = Cursors.Arrow;
                    break;
                case input.CursorType.Hand:
                    Cursor = Cursors.Hand;
                    break;
                case input.CursorType.HandGrab:
                    Cursor = Cursors.ScrollAll;
                    break;
                case input.CursorType.Cross:
                    Cursor = Cursors.Cross;
                    break;
                default:
                    throw new Exception();
            }
        }

        private void MyMapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Handled)
            {
                return;
            }

            e.MouseDevice.Capture(null);

            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

            if (_isLeftClick == true)
            {
                MapListener?.LeftClick(_lastMapInfo);

                _isLeftClick = false;
            }

            _isLeftMouseDown = false;
        }

        private void MyMapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (++_counter > 1)
            {
                _isLeftClick = false;
            }

            base.OnMouseMove(e);

            if (e.Handled == true)
            {
                return;
            }

            var args = e.ToMouseEventArgs(this);
            Controller.HandleMouseMove(this, args);

            if (args.Handled == false)
            {
                if (_isLeftMouseDown == true && e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {

                    SetCursor(input.CursorType.HandGrab);
                }
                else
                {
                    SetCursor(input.CursorType.Default);
                }
            }
        }

        private void MyMapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _counter = 0;

            base.OnMouseDown(e);
            if (e.Handled)
            {
                return;
            }

            Focus();
            e.MouseDevice.Capture(this);

            // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
            //  _mouseDownPoint = e.GetPosition(this).ToScreenPoint();

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                _isLeftMouseDown = true;
            }

            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            Controller.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            Controller.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.Handled)
            {
                return;
            }

            _isLeftMouseDown = false;

            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public Mapsui.Geometries.Point ScreenToWorld(Mapsui.Geometries.Point screenPosition)
        {
            return Viewport.ScreenToWorld(screenPosition);
        }

        public Mapsui.Geometries.Point WorldToScreen(Mapsui.Geometries.Point worldPosition)
        {
            return Viewport.WorldToScreen(worldPosition);
        }
    }
}
