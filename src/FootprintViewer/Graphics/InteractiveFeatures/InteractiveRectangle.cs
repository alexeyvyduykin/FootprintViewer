using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class InteractiveRectangle : InteractiveFeature
    {
        private bool _isDrawing = false;

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
            throw new System.NotImplementedException();
        }

        public override bool Dragging(Point worldPosition)
        {
            throw new System.NotImplementedException();
        }

        public override void EndDragging()
        {
            throw new System.NotImplementedException();
        }
        //public bool StartDragging(MapInfo mapInfo, double screenDistance)
        //{
        //    if (mapInfo.Feature != null && mapInfo.Feature is InteractiveFeature interactiveFeature)
        //    {
        //        var vertexTouched = FindVertexTouched(mapInfo, interactiveFeature.EditVertices(), screenDistance);
        //        if (vertexTouched != null)
        //        {
        //            _dragInfo.Feature = interactiveFeature;
        //            _dragInfo.Vertex = vertexTouched;
        //            _dragInfo.StartOffsetToVertex = mapInfo.WorldPosition - _dragInfo.Vertex;

        //            return true; // to indicate start of drag
        //        }
        //    }

        //    return false;
        //}

        //public bool Dragging(Point worldPosition)
        //{
        //    if (_dragInfo.Feature == null)
        //        return false;

        //    SetPointXY(_dragInfo.Vertex, worldPosition - _dragInfo.StartOffsetToVertex);

        //    if (_dragInfo.Feature.Geometry is Polygon polygon) // Not this only works correctly it the feature is in the outerring.
        //    {
        //        var count = polygon.ExteriorRing.Vertices.Count;
        //        var vertices = polygon.ExteriorRing.Vertices;
        //        var index = vertices.IndexOf(_dragInfo.Vertex);
        //        if (index >= 0)
        //        {
        //            // It is a ring where the first should be the same as the last.
        //            // So if the first was removed than set the last to the value of the new first
        //            if (index == 0)
        //            {
        //                SetPointXY(vertices[count - 1], vertices[0]);
        //            }
        //            // If the last was removed then set the first to the value of the new last
        //            else if (index == vertices.Count)
        //            {
        //                SetPointXY(vertices[0], vertices[count - 1]);
        //            }
        //        }
        //    }

        //    _dragInfo.Feature.RenderedGeometry.Clear();
        //    Layer.DataHasChanged();
        //    return true;
        //}

        //public void StopDragging()
        //{
        //    if (_dragInfo.Feature != null)
        //    {
        //        _dragInfo.Feature = null;
        //    }
        //}

        //private Point FindVertexTouched(MapInfo mapInfo, IEnumerable<Point> vertices, double screenDistance)
        //{
        //    return vertices.OrderBy(v => v.Distance(mapInfo.WorldPosition)).FirstOrDefault(v => v.Distance(mapInfo.WorldPosition) < mapInfo.Resolution * screenDistance);
        //}

        //private void SetPointXY(Point target, Point position)
        //{
        //    target.X = position.X;
        //    target.Y = position.Y;
        //}
    }
}
