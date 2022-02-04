﻿using Mapsui.Geometries;

namespace FootprintViewer
{
    public class DrawingManipulator2 : MouseManipulator
    {
        public DrawingManipulator2(IMapView view) : base(view) { }

        private bool _skip;
        private int _counter;
        private const int _minPixelsMovedForDrag = 4;

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_skip == false)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.ScreenToWorld(screenPosition);

                bool isClick(Point worldPosition)
                {
                    var p0 = MapView.WorldToScreen(worldPosition);

                    var res = IsClick(p0, screenPosition);

                    if (res == true)
                    {
                        MapView.SetCursor(CursorType.Default);
                    }

                    return res;
                }

                MapView.MapObserver.OnCompleted(worldPosition, isClick);         
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

        private static bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }

    public class HoverDrawingManipulator2 : MouseManipulator
    {
        public HoverDrawingManipulator2(IMapView view) : base(view)
        {

        }

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