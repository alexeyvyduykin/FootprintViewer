using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.InteractivityEx
{
    public class InteractivePolygon : InteractiveFeature
    {
        private bool _isDrawing = false;

        private bool _isEditing = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        public InteractivePolygon(IFeature feature) : base(feature) { }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                if (_isDrawing == true)
                {
                    return ((LineString)Geometry).Vertices;
                }
                else
                {
                    return ((Polygon)Geometry).ExteriorRing.Vertices;
                }
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

            var position = worldPosition - _startOffsetToVertex;
            
            _vertex.X = position.X;
            _vertex.Y = position.Y;

            //if (Geometry is Polygon polygon) // Not this only works correctly it the feature is in the outerring.
            //{
            //    var count = polygon.ExteriorRing.Vertices.Count;
            //    var vertices = polygon.ExteriorRing.Vertices;
            //    var index = vertices.IndexOf(_vertex);

            //    if (index >= 0)
            //    {
            //        // It is a ring where the first should be the same as the last.
            //        // So if the first was removed than set the last to the value of the new first
            //        if (index == 0)
            //        {
            //            vertices[count - 1].X = vertices[0].X;
            //            vertices[count - 1].Y = vertices[0].Y;
            //        }
            //        // If the last was removed then set the first to the value of the new last
            //        else if (index == vertices.Count)
            //        {
            //            vertices[0].X = vertices[count - 1].X;
            //            vertices[0].Y = vertices[count - 1].Y;
            //        }
            //    }
            //}

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
