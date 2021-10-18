using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
using NetTopologySuite.GeometriesGraph;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class InteractivePolygon : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Feature _helpLineString;
        private Feature _helpPolygon;

        private bool _isDragging = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

        public InteractivePolygon() : base() { }

        public InteractivePolygon(IFeature feature) : base(feature) { }

        public override AddInfo BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                return new AddInfo();
            }

            _isDrawing = true;

            var p0 = worldPosition.Clone();
            // Add a second point right away. The second one will be the 'hover' vertex
            var p1 = worldPosition.Clone();

            Geometry = new LineString(new[] { p0 });
            this["Name"] = FeatureType.AOIPolygonBorderDrawing.ToString();

            _helpLineString = new Feature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = FeatureType.AOIPolygonDrawing.ToString(),
            };

            _helpPolygon = new Feature
            {
                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0 })
                },
                ["Name"] = FeatureType.AOIPolygonAreaDrawing.ToString(),
            };

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>() { _helpLineString, _helpPolygon },
            };
        }

        public override void Drawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_helpLineString.Geometry).EndPoint;
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();

                ((Polygon)_helpPolygon.Geometry).ExteriorRing.Vertices.Add(p0);
                ((LineString)Geometry).Vertices.Add(p0); // and add it to the geometry
                ((LineString)_helpLineString.Geometry).Vertices = new[] { p1, p2 };

                RenderedGeometry?.Clear();
                _helpLineString.RenderedGeometry?.Clear();
                _helpPolygon.RenderedGeometry?.Clear();
            }
        }

        public override void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_helpLineString.Geometry).EndPoint.X = worldPosition.X;
                ((LineString)_helpLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _helpLineString.RenderedGeometry?.Clear();
            }
        }

        public override void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var vertices = ((LineString)Geometry).Vertices;

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                this["Name"] = FeatureType.AOIPolygon.ToString();
            }
        }

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

        public override void EndDragging()
        {
            if (_isDragging == true)
            {
                _isDragging = false;
            }
        }
    }
}
