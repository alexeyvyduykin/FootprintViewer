using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
//using NetTopologySuite.GeometriesGraph;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class InteractiveRoute : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Feature _helpLineString;

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

            this["Name"] = FeatureType.Route.ToString();

            _helpLineString = new Feature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = FeatureType.RouteDrawing.ToString(),
            };

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>() { _helpLineString },
            };
        }

        public override void Drawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_helpLineString.Geometry).EndPoint;
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();

                ((LineString)Geometry).Vertices.Add(p0); // and add it to the geometry
                ((LineString)_helpLineString.Geometry).Vertices = new[] { p1, p2 };

                RenderedGeometry?.Clear();
                _helpLineString.RenderedGeometry?.Clear();
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
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                return ((LineString)Geometry).Vertices;
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
