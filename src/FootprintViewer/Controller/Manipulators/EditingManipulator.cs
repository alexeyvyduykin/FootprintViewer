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
        private int _vertexRadius = 100;
        private InteractiveFeature _draggingFeature;
        private WritableLayer _layer = null;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {        
            base.Completed(e);

            if(_isDragging == true)
            {
                _draggingFeature.EndDragging();

                var bb = _draggingFeature.Geometry.BoundingBox;
                             
                MapView.Map.PanLock = false;

                MapView.NavigateToAOI(bb);

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

                _draggingFeature.Dragging(worldPosition);

                MapView.SetCursorType(CursorType.FeatureDragging);
                _layer.DataHasChanged();                
            }
            
            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {       
            base.Started(e);
    
            var screenPosition = e.Position;
            var mapInfo = MapView.GetMapInfo(screenPosition);

            _isDragging = StartDragging(mapInfo, _vertexRadius);

            if(_isDragging == true)
            {
                MapView.Map.PanLock = true;          
            }

            e.Handled = true;
        }

        private bool StartDragging(MapInfo mapInfo, double screenDistance)
        {
            if (mapInfo.Feature != null && mapInfo.Feature is InteractiveFeature interactiveFeature)
            {
                var distance = mapInfo.Resolution * screenDistance;

                _draggingFeature = interactiveFeature;
                _layer = (WritableLayer)mapInfo.Layer;

                return interactiveFeature.BeginDragging(mapInfo.WorldPosition, distance);
            }

            return false;
        }
    }
}
