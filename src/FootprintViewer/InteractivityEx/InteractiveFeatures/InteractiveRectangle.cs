using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.InteractivityEx
{
    public class InteractiveRectangle : InteractiveFeature
    {
        private bool _isDrawing = false;

        private bool _isEditing = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        public InteractiveRectangle(IFeature feature) : base(feature) { }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null && _isDrawing == false)
            {
                return ((Polygon)Geometry).ExteriorRing.Vertices;
            }

            return new List<Point>();
        }

        public override bool BeginEditing(Point worldPosition, double screenDistance)
        {
            if (_isEditing == true)
            {
                return false;
            }

            var vertices = EditVertices();

            var vertexTouched = vertices.OrderBy(v => v.Distance(worldPosition)).FirstOrDefault(v => v.Distance(worldPosition) < screenDistance);
            
            if (vertexTouched != null)
            {
                _vertex = vertexTouched;
                _startOffsetToVertex = worldPosition - vertexTouched;
                _isEditing = true;

                return true; // to indicate start of drag
            }

            return false;
        }

        public override bool Editing(Point worldPosition)
        {
            if (_isEditing == false)
            {
                return false;
            }
     
            if (Geometry is Polygon polygon) // Not this only works correctly it the feature is in the outerring.
            {
                var position = worldPosition - _startOffsetToVertex;

                var vertices = polygon.ExteriorRing.Vertices;                            
                var index = vertices.IndexOf(_vertex);

                var i0 = (index - 1 >= 0) ? index - 1 : 3;
                var i1 = (index + 1 < 4) ? index + 1 : 0;

                if (vertices[i0].X == _vertex.X) // horizontal edge
                {
                    _vertex.X = position.X;
                    _vertex.Y = position.Y;

                    vertices[i0].X = position.X;
                    vertices[i1].Y = position.Y;
                }
                else
                {
                    _vertex.X = position.X;
                    _vertex.Y = position.Y;

                    vertices[i0].Y = position.Y;
                    vertices[i1].X = position.X;
                }
            }

            RenderedGeometry.Clear();

            return true;
        }

        public override void EndEditing()
        {
            if (_isEditing == true)
            {
                _isEditing = false;
            }
        }
    }
}
