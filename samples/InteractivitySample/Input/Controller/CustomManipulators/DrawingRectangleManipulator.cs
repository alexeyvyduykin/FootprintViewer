using InteractivitySample.Input.Controller.Core;

namespace InteractivitySample.Input.Controller
{
    public class DrawingRectangleManipulator : MouseManipulator
    {
        public DrawingRectangleManipulator(IMapView mapView) : base(mapView) { }

        private bool _skip;
        private int _counter;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.ScreenToWorld(screenPosition);

                MapView.MapObserver.OnCompleted(worldPosition);
                                  
                MapView.SetCursor(CursorType.Default);                
            }

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.MapObserver.OnDelta(worldPosition);

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

            var screenPosition = e.Position;
            var worldPosition = MapView.ScreenToWorld(screenPosition);

            MapView.MapObserver.OnStarted(worldPosition, 0);

            e.Handled = true;
        }
    }

    public class HoverDrawingRectangleManipulator : MouseManipulator
    {
        public HoverDrawingRectangleManipulator(IMapView plotView) : base(plotView) { }

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
