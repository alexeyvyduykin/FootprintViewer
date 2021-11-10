using Mapsui.Geometries;
using System;

namespace FootprintViewer
{
    public class DrawingPolygonManipulator : MouseManipulator
    {
        public DrawingPolygonManipulator(IMapView view) : base(view)
        {
        }

        private bool _skip;
        private int _counter;
        private const int _minPixelsMovedForDrag = 4;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                bool isClick(Point worldPosition)
                {
                    var p0 = MapView.Viewport.WorldToScreen(worldPosition);

                    return IsClick(p0, screenPosition);
                }

                var (isDraw, bb) = MapView.Plotter.CreatingFeature(worldPosition, isClick);

                if (isDraw == true)
                {
                    MapView.NavigateToAOI(bb);

                    MapView.SetCursor(CursorType.Default, "DrawingPolygonManipulator.Completed");
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

        private static bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }

    public class HoverDrawingPolygonManipulator : MouseManipulator
    {
        public HoverDrawingPolygonManipulator(IMapView view) : base(view)
        {

        }

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

            MapView.SetCursor(CursorType.Cross, "HoverDrawingPolygonManipulator.Started");
            
            e.Handled = true;
        }
    }
}