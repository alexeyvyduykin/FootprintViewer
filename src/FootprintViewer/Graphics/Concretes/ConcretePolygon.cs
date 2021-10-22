using Mapsui;
using System;
using System.Collections.Generic;
using System.Text;
using Mapsui.Geometries;
namespace FootprintViewer.Graphics
{
    public class ConcretePolygon : Concrete
    {
        private InteractivePolygon _polygon;
        private readonly int _minPixelsMovedForDrag = 4;

        public override IInteractiveFeature CreateConcrete()
        {
            _polygon = new InteractivePolygon(null);
            return _polygon;
        }
        public override bool IsEndDrawing(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_polygon != null)
            {
                var polygonGeometry = (LineString)_polygon.Geometry;

                if (polygonGeometry.Vertices.Count > 2)
                {
                    // is end?
                    var p0 = viewport.WorldToScreen(polygonGeometry.Vertices[0]);

                    bool click = IsClick(p0, screenPosition);

                    if (click == true)
                    {
                        return true;
                    }
                }

                return false;
            }

            throw new Exception();
        }

        private bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }
}
