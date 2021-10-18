using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FootprintViewer
{
    public class EditingManipulator : MouseManipulator
    {
        private bool _isDragging = false;    
        private int _vertexRadius = 50;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {        
            base.Completed(e);

            if(_isDragging == true)
            {
                MapView.EditManager.StopDragging();

                MapView.Map.PanLock = false;
                _isDragging = false;
            }
          
            MapView.SetCursorType(CursorType.Default);

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {                    
            base.Delta(e);

            if (_isDragging == true)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                MapView.EditManager.Dragging(worldPosition);
            }
            
            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {       
            base.Started(e);
    
            var screenPosition = e.Position;
            var mapInfo = MapView.GetMapInfo(screenPosition);

            _isDragging = MapView.EditManager.StartDragging(mapInfo, _vertexRadius);

            if(_isDragging == true)
            {
                MapView.Map.PanLock = true;

                MapView.SetCursorType(GetCursorType());
            }

            e.Handled = true;
        }

        private CursorType GetCursorType()
        {
            return CursorType.ZoomRectangle;
        }
    }
}
