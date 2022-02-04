﻿#nullable enable

using Mapsui.Geometries;

namespace FootprintViewer
{
    public class DrawingRouteManipulator : MouseManipulator
    {
        private bool _skip;
        private int _counter;
        private readonly int _minPixelsMovedForDrag = 4;
        public DrawingRouteManipulator(IMapView plotView) : base(plotView) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                bool IsEnd(Point worldPosition)
                {                 
                    var p = MapView.Viewport.WorldToScreen(worldPosition);

                    return IsClick(p, screenPosition);                    
                }

                var (isCreate, _) = MapView.Plotter.CreatingFeature(worldPosition, IsEnd);

                if (isCreate == true)
                {
                    MapView.SetCursor(CursorType.Default, "DrawingRouteManipulator.Completed");
                }
            }
 
            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (_counter++ > 0)
            {
                _skip = true;
            }
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            _skip = false;
            _counter = 0;
                
            e.Handled = true;            
        }

        private bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }

    public class HoverDrawingLineManipulator : MouseManipulator
    {
        public HoverDrawingLineManipulator(IMapView plotView) : base(plotView) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

            MapView.Plotter.HoverCreatingFeature(worldPosition);
            
            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            MapView.SetCursor(CursorType.Cross, "HoverDrawingLineManipulator.Started");

            e.Handled = true;
        }
    }
}