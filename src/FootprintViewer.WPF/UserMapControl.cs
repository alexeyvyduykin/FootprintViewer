﻿using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.UI.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FootprintViewer.WPF
{
    public class UserMapControl : MapControl, IMapView
    {
        private Mapsui.Geometries.Point? _mouseDownPoint;      
        private readonly EditManager _editManager;
        private IController _actualController;

        public UserMapControl() : base()
        {
            _editManager = new EditManager();

            _actualController = new EditController();

            MouseEnter += MyMapControl_MouseEnter;
            MouseLeave += MyMapControl_MouseLeave;
            MouseWheel += MyMapControl_MouseWheel;
            MouseDown += MyMapControl_MouseDown;
            MouseMove += MyMapControl_MouseMove;
            MouseUp += MyMapControl_MouseUp;
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
            _actualController.HandleMouseUp(this, e.ToMouseReleasedEventArgs(this));

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
            _actualController.HandleMouseMove(this, e.ToMouseEventArgs(this));
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
            _actualController.HandleMouseDown(this, e.ToMouseDownEventArgs(this));
        }

        private void MyMapControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Handled /*|| !IsMouseWheelEnabled*/)
            {
                return;
            }

            //e.Handled = 
            _actualController.HandleMouseWheel(this, e.ToMouseWheelEventArgs(this));
        }

        private void MyMapControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (e.Handled)
            {
                return;
            }

            //e.Handled = 
            _actualController.HandleMouseLeave(this, e.ToMouseEventArgs(this));
        }

        private void MyMapControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.Handled)
            {
                return;
            }

            //e.Handled = 
            _actualController.HandleMouseEnter(this, e.ToMouseEventArgs(this));
        }

        public EditManager EditManager => _editManager;

        public void NavigateToAOI(BoundingBox boundingBox)
        {
            var layer = (WritableLayer)Map.Layers.First(l => l.Name == nameof(LayerType.EditLayer));
       
            var features = layer.GetFeatures().ToList();
          
            // HACK: maybe problems in future
            if (features.Count > 1)
            {              
                var f = features.Where(s => s.Geometry.BoundingBox.Equals(boundingBox) == false).Single();

                layer.TryRemove(f);                
            }

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

        public void SetActiveTool(ToolType tool)
        {
            switch (tool)
            {
                case ToolType.None:
                    _actualController = null;
                    break;
                case ToolType.DrawingRectangleAOI:
                    _actualController = new DrawRectangleController();
                    break;
                case ToolType.DrawingPolygonAOI:
                    _actualController = new DrawPolygonController();
                    break;
                case ToolType.DrawingCircleAOI:
                    _actualController = new DrawCircleController();
                    break;
                case ToolType.RoutingDistance:
                    _actualController = new DrawRouteController();
                    break;
                case ToolType.Editing:
                    _actualController = new EditController();
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
