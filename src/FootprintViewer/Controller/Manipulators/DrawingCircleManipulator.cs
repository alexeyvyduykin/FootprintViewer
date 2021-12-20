namespace FootprintViewer
{
    public class DrawingCircleManipulator : MouseManipulator
    {
        public DrawingCircleManipulator(IMapView view) : base(view) { }

        private bool _skip;
        private int _counter;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                var (isDraw, bb) = MapView.Plotter.CreatingFeature(worldPosition);

                if (isDraw == true)
                {
                    MapView.NavigateToAOI(bb);
                    MapView.SetCursor(CursorType.Default, "DrawingCircleManipulator.Completed");
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
    }

    public class HoverDrawingCircleManipulator : MouseManipulator
    {
        public HoverDrawingCircleManipulator(IMapView view) : base(view)
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

            MapView.SetCursor(CursorType.Cross, "HoverDrawingCircleManipulator.Started");

            e.Handled = true;
        }
    }
}