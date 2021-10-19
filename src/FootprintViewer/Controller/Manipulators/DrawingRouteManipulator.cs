#nullable enable

namespace FootprintViewer
{
    public class DrawingRouteManipulator : MouseManipulator
    {
        private bool _skip;
        private int _counter;

        public DrawingRouteManipulator(IMapView plotView) : base(plotView) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                MapView.Observer.DrawingRoute(worldPosition, screenPosition, MapView.Viewport);
            }

            MapView.SetCursorType(CursorType.Default);

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

            //if (_editMode == EditMode.AddLine)
            {
                MapView.SetCursorType(GetCursorType());
                e.Handled = true;
            }
        }

        private CursorType GetCursorType()
        {
            return CursorType.ZoomRectangle;
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

            MapView.Observer.DrawingHoverRoute(worldPosition);
        }
    }
}