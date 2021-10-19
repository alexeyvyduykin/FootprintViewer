using Mapsui.Geometries;
using Mapsui.Geometries.Utilities;
using Mapsui.Projection;
using Mapsui.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
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
                var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

                var (isDraw, bb, f) = MapView.Observer.CreatingRectangle(worldPosition);
                
                if (isDraw == true)
                {
                    var coord = ProjectHelper.ToString(bb.Centroid);
                    var vertices = f.Geometry.AllVertices().Select(s => SphericalMercator.ToLonLat(s.X, s.Y)).ToArray();                    
                    var area = SphericalUtil.ComputeSignedArea(vertices);
                    string str = $"{area:N2} km² | {coord}";
                    MapView.SetDescriptionAOI(str);
                    MapView.NavigateToAOI(bb);
                }
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

            MapView.SetCursorType(GetCursorType());
                
            e.Handled = true;            
        }

        private CursorType GetCursorType()
        {
            return CursorType.ZoomRectangle;
        }
    }

    public class HoverDrawingRectangleManipulator : MouseManipulator
    {
        public HoverDrawingRectangleManipulator(IMapView plotView) : base(plotView) { }

        public override void Delta(MouseEventArgs e)
        {
            base.Delta(e);

            var screenPosition = e.Position;
            var worldPosition = MapView.Viewport.ScreenToWorld(screenPosition);

            MapView.Observer.HoverCreatingRectangle(worldPosition);
        }
    }

}
