using InteractivitySample.Layers;
using Mapsui.Layers;
using InteractivitySample.Input.Controller.Core;

namespace InteractivitySample.Input.Controller
{
    public class EditingManipulator : MouseManipulator
    {
        private bool _isEditing = false;
        private readonly int _vertexRadius = 4;
        private ILayer? _layer = null;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {
            base.Completed(e);

            if (_isEditing == true)
            {
                var worldPosition = MapView.ScreenToWorld(e.Position);

                MapView.MapObserver.OnCompleted(worldPosition);

                MapView.Map.PanLock = false;

                _isEditing = false;
            }

            MapView.SetCursor(CursorType.Default);

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (_isEditing == true)
            {
                var worldPosition = MapView.ScreenToWorld(e.Position);

                MapView.MapObserver.OnDelta(worldPosition);

                MapView.SetCursor(CursorType.HandGrab);

                _layer?.DataHasChanged();

                e.Handled = true;
            }

            //e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {
            base.Started(e);

            var mapInfo = MapView.GetMapInfo(e.Position);

            _isEditing = false;

            if (mapInfo.Feature != null && mapInfo.Layer is InteractiveLayer)
            {
                var distance = mapInfo.Resolution * _vertexRadius;

                _layer = mapInfo.Layer;

                MapView.MapObserver.OnStarted(mapInfo.WorldPosition, distance);

                _isEditing = true;
            }

            if (_isEditing == true)
            {
                MapView.Map.PanLock = true;
            }

            e.Handled = true;
        }

    }

    public class HoverEditingManipulator : MouseManipulator
    {
        public HoverEditingManipulator(IMapView view) : base(view) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            if (e.Handled == false)
            {
                var mapInfo = MapView.GetMapInfo(e.Position);

                if (mapInfo.Layer != null && mapInfo.Layer is InteractiveLayer)
                {
                    MapView.SetCursor(CursorType.Hand);
                    e.Handled = true;
                }
            }
        }
    }
}
