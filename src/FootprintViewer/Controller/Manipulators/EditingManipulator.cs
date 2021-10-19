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
        private bool _isEditing = false;    
        private int _vertexRadius = 4;
        private InteractiveFeature _editingFeature;
        private EditLayer _layer = null;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {        
            base.Completed(e);

            if(_isEditing == true)
            {
                _editingFeature.EndEditing();

                var bb = _editingFeature.Geometry.BoundingBox;
                             
                MapView.Map.PanLock = false;

                MapView.NavigateToAOI(bb);

                _isEditing = false;
            }

            MapView.SetCursorType(CursorType.Default);

            e.Handled = true;
        }

        public override void Delta(MouseEventArgs e)
        {                    
            base.Delta(e);

            if (_isEditing == true)
            {
                var screenPosition = e.Position;
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                _editingFeature.Editing(worldPosition);

                MapView.SetCursorType(CursorType.EditingFeaturePoint);
                _layer.DataHasChanged();                
            }
            
            e.Handled = true;
        }

        public override void Started(MouseEventArgs e)
        {       
            base.Started(e);
    
            var screenPosition = e.Position;
            var mapInfo = MapView.GetMapInfo(screenPosition);

            _isEditing = StartDragging(mapInfo, _vertexRadius);

            if(_isEditing == true)
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

                _editingFeature = interactiveFeature;
                _layer = (EditLayer)mapInfo.Layer;

                return interactiveFeature.BeginEditing(mapInfo.WorldPosition, distance);
            }

            return false;
        }
    }
}
