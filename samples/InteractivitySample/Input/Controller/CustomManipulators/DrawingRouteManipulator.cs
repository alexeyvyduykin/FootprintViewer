using InteractivitySample.Input.Controller.Core;
using Mapsui.Geometries;

namespace InteractivitySample.Input.Controller
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
                var worldPosition = MapView.ScreenToWorld(screenPosition);

                bool IsEnd(Point worldPosition)
                {                 
                    var p = MapView.WorldToScreen(worldPosition);

                    return IsClick(p, screenPosition);                    
                }

                MapView.MapObserver.OnCompleted(worldPosition, IsEnd);
                    
                MapView.SetCursor(CursorType.Default);                
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
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.MapObserver.OnHover(worldPosition);
            
            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            MapView.SetCursor(CursorType.Cross);

            e.Handled = true;
        }
    }
}