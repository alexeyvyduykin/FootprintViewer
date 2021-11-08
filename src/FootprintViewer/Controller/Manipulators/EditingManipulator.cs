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
        private EditLayer _layer = null;

        public EditingManipulator(IMapView mapView) : base(mapView) { }

        public override void Completed(MouseEventArgs e)
        {        
            base.Completed(e);

            if(_isEditing == true)
            {
                var (_, bb) = MapView.Plotter.EndEditingFeature();
                             
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

                MapView.Plotter.EditingFeature(worldPosition);

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

            _isEditing = false;

            if (mapInfo.Feature != null && mapInfo.Feature is InteractiveFeature interactiveFeature)
            {
                var distance = mapInfo.Resolution * _vertexRadius;

                _layer = (EditLayer)mapInfo.Layer;

                MapView.Plotter.BeginEditingFeature(mapInfo.WorldPosition, distance);
               
                _isEditing = true;
            }
                                
            if(_isEditing == true)
            {
                MapView.Map.PanLock = true;          
            }

            e.Handled = true;
        }
    }
}
