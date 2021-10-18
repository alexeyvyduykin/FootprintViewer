using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
using NetTopologySuite.Triangulate.QuadEdge;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class InteractiveRectangle : InteractiveFeature
    {
        private bool _isDrawing = false;

        private bool _isDragging = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        public override AddInfo BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                var p0 = worldPosition.Clone();
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();
                var p3 = worldPosition.Clone();

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0, p1, p2, p3 })
                };

                this["Name"] = FeatureType.AOIRectangleDrawing.ToString();
            }

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>(),
            };
        }

        public override void Drawing(Point worldPosition)
        {
            EndDrawing();
        }

        public override void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p2 = worldPosition.Clone();
                var rectangle = (Polygon)Geometry;
                var p0 = rectangle.ExteriorRing.Vertices[0];

                var p1 = new Point(p2.X, p0.Y);
                var p3 = new Point(p0.X, p2.Y);

                ((Polygon)Geometry).ExteriorRing.Vertices[0] = p0;
                ((Polygon)Geometry).ExteriorRing.Vertices[1] = p1;
                ((Polygon)Geometry).ExteriorRing.Vertices[2] = p2;
                ((Polygon)Geometry).ExteriorRing.Vertices[3] = p3;

                RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public override void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var vertices = ((Polygon)Geometry).ExteriorRing.Vertices;

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                this["Name"] = FeatureType.AOIRectangle.ToString();

                RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null && _isDrawing == false)
            {
                return ((Polygon)Geometry).ExteriorRing.Vertices;
            }

            return new List<Point>();
        }

        public override bool BeginDragging(Point worldPosition, double screenDistance)
        {
            if (_isDragging == true)
            {
                return false;
            }

            var vertices = EditVertices();

            var vertexTouched = vertices.OrderBy(v => v.Distance(worldPosition)).FirstOrDefault(v => v.Distance(worldPosition) < screenDistance);
            
            if (vertexTouched != null)
            {
                _vertex = vertexTouched;
                _startOffsetToVertex = worldPosition - vertexTouched;
                _isDragging = true;

                return true; // to indicate start of drag
            }

            return false;
        }

        public override bool Dragging(Point worldPosition)
        {
            if (_isDragging == false)
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

        public override void EndDragging()
        {
            if (_isDragging == true)
            {
                _isDragging = false;
            }
        }
    }
}
