using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.UI.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using FootprintViewer.Graphics;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using FootprintViewer.WPF.Controls;
using System.Collections.ObjectModel;
using FootprintViewer.ViewModels;
using System.Security.Policy;
using System.Diagnostics;

namespace FootprintViewer.WPF
{
    public class UserMapControl : MapControl, IMapView
    {
        private Mapsui.Geometries.Point? _mouseDownPoint;
        private readonly TipControl _tipOverlay;
        private bool _isActivateTip = false;
        private Cursor? _grabHandCursor;
        private bool _isLeftMouseDown = false;
        private CursorType _currentCursorType = CursorType.Default;
        public UserMapControl() : base()
        {
            MouseEnter += MyMapControl_MouseEnter;
            MouseLeave += MyMapControl_MouseLeave;
            MouseWheel += MyMapControl_MouseWheel;
            MouseDown += MyMapControl_MouseDown;
            MouseMove += MyMapControl_MouseMove;
            MouseUp += MyMapControl_MouseUp;

            _tipOverlay = new TipControl();
            Children.Add(_tipOverlay);
        }

        public Plotter Plotter
        {
            get { return (Plotter)GetValue(PlotterProperty); }
            set { SetValue(PlotterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Observer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlotterProperty =
            DependencyProperty.Register("Plotter", typeof(Plotter), typeof(UserMapControl));

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
            mapControl.Controller.HandleMouseEnter(mapControl, new MouseEventArgs());
        }

        public Tip? TipSource
        {
            get { return (Tip)GetValue(TipSourceProperty); }
            set { SetValue(TipSourceProperty, value); }
        }

        public static readonly DependencyProperty TipSourceProperty =
            DependencyProperty.Register("TipSource", typeof(Tip), typeof(UserMapControl), new PropertyMetadata(null, OnTipSourceChanged));

        private static void OnTipSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mapControl = (UserMapControl)d;
            if (e.NewValue == null)
            {
                mapControl.HideTip();
            }                
            else
            {            
                mapControl.ActiveateTip();         
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

        private void MyMapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Handled)
            {
                return;
            }

            //var releasedArgs = (PointerReleasedEventArgs)e;
            
            e.MouseDevice.Capture(null);
            
            _isLeftMouseDown = false;

            //e.Handled = 
            Controller.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

            // Open the context menu
            //var p = e.GetPosition(this).ToScreenPoint();
            //var d = p.DistanceTo(_mouseDownPoint);

            //if (ContextMenu != null)
            //{
            //    if (Math.Abs(d) < 1e-8 && releasedArgs.InitialPressMouseButton == MouseButton.Right)
            //    {
            //        ContextMenu.DataContext = DataContext;
            //        ContextMenu.IsVisible = true;
            //    }
            //    else
            //    {
            //        ContextMenu.IsVisible = false;
            //    }
            //}
        }

        private void MyMapControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
  
            if (e.Handled)
            {
                return;
            }

            if (TipSource != null)
            {
                if (_isActivateTip == true)
                {
                    _tipOverlay.ItemsSource = new ObservableCollection<Tip>() { TipSource };
                    _isActivateTip = false;
                }

                var screenPosition = e.GetPosition(this);
                TipSource.X = screenPosition.X + 20;
                TipSource.Y = screenPosition.Y;
            }

            //e.Handled = 

            var args = e.ToMouseEventArgs(this);
            Controller.HandleMouseMove(this, args);

            if (args.Handled == false)
            {
                if (_isLeftMouseDown == true && e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    SetCursor(CursorType.HandGrab, "UserMapControl.MyMapControl_MouseMove");                   
                }
                else
                {
                    SetCursor(CursorType.Default, "UserMapControl.MyMapControl_MouseMove");
                }
            }
        }

        private void MyMapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            base.OnMouseDown(e);
            if (e.Handled)
            {
                return;
            }

            Focus();
            e.MouseDevice.Capture(this);

            // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
            _mouseDownPoint = e.GetPosition(this).ToScreenPoint();

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                _isLeftMouseDown = true;
            }

            //e.Handled = 
            Controller.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            //e.Handled = 
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

            //e.Handled = 
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

            //e.Handled = 
            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public void NavigateToAOI(BoundingBox boundingBox)
        {                
            Navigator.NavigateTo(boundingBox.Grow(boundingBox.Width * 0.2));            
        }

        public void SetCursor(CursorType cursorType, string info = "")
        {
            if (_grabHandCursor == null)
            {
                _grabHandCursor = new Cursor(App.GetResourceStream(new Uri("resources/GrabHand.cur", UriKind.Relative)).Stream);
            }

            if (_currentCursorType == cursorType)
            {
                return;
            }

            switch (cursorType)
            {
                case CursorType.Default:
                    Cursor = Cursors.Arrow;
                    break;
                case CursorType.Hand:
                    Cursor = Cursors.Hand;
                    break;
                case CursorType.HandGrab:
                    Cursor = _grabHandCursor;
                    break;
                case CursorType.Cross:
                    Cursor = Cursors.Cross;
                    break;
                default:
                    throw new Exception();
            }

            _currentCursorType = cursorType;

            Debug.WriteLine($"Set Cursor = {Cursor}, Info = {info}");
        }
        
        protected void ActiveateTip()
        {
            _isActivateTip = true;     
        }

        protected void HideTip()
        {       
            _tipOverlay.ItemsSource = new ObservableCollection<Tip>();                        
        }

        public void InvalidatePlot(bool updateData = true)
        {
            base.InvalidateVisual();
        }
    }
}
