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

namespace FootprintViewer.WPF
{
    public class UserMapControl : MapControl, IMapView
    {
        private Mapsui.Geometries.Point? _mouseDownPoint;          
     
        public UserMapControl() : base()
        {                 
            MouseEnter += MyMapControl_MouseEnter;
            MouseLeave += MyMapControl_MouseLeave;
            MouseWheel += MyMapControl_MouseWheel;
            MouseDown += MyMapControl_MouseDown;
            MouseMove += MyMapControl_MouseMove;
            MouseUp += MyMapControl_MouseUp;
        }

        public IInteractiveFeatureObserver Observer
        {
            get { return (IInteractiveFeatureObserver)GetValue(ObserverProperty); }
            set { SetValue(ObserverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Observer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ObserverProperty =
            DependencyProperty.Register("Observer", typeof(IInteractiveFeatureObserver), typeof(UserMapControl));

        public IController Controller
        {
            get { return (IController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IController), typeof(UserMapControl), new PropertyMetadata(new EditController()));


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

            //e.Handled = 
            Controller.HandleMouseMove(this, e.ToMouseEventArgs(this));
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

            //e.Handled = 
            Controller.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public void NavigateToAOI(BoundingBox boundingBox)
        {              
            Navigator.NavigateTo(boundingBox.Grow(boundingBox.Width * 0.2));
        }

        public void SetCursorType(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Pan:
                    Cursor = Cursors.Hand;
                    break;
                case CursorType.ZoomRectangle:
                    Cursor = Cursors.SizeAll;
                    break;
                case CursorType.ZoomHorizontal:
                    Cursor = Cursors.SizeWE;
                    break;
                case CursorType.ZoomVertical:
                    Cursor = Cursors.SizeNS;
                    break;

                case CursorType.HoverEditPoint:
                    Cursor = Cursors.UpArrow;
                    break;
                case CursorType.HoverDragPoint:
                    Cursor = Cursors.SizeAll;
                    break;

                case CursorType.EditingFeaturePoint:
                    Cursor = Cursors.SizeAll;
                    break;
                case CursorType.DraggingFeature:
                    Cursor = Cursors.SizeAll;
                    break;

                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        public void HideTracker()
        {
            throw new NotImplementedException();
        }

        public void InvalidatePlot(bool updateData = true)
        {
            base.InvalidateVisual();
        }

        public void SetClipboardText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
