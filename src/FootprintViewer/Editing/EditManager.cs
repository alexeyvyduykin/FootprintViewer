using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.UI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class EditManager
    {
        public WritableLayer Layer { get; set; }

        private AddInfo? _addInfo;
        private DragInfo _dragInfo = new DragInfo();

        private readonly int _minPixelsMovedForDrag = 4;

        public (bool, BoundingBox) DrawingRectangle(Point worldPosition)
        {
            if (_addInfo == null)
            {
                var interactiveRectangle = new InteractiveRectangle();

                _addInfo = interactiveRectangle.BeginDrawing(worldPosition);

                Layer.Add(_addInfo.Feature);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                _addInfo.Feature.EndDrawing();

                BoundingBox bb = _addInfo.Feature.Geometry.BoundingBox;

                _addInfo = null;

                Layer.DataHasChanged();

                return (true, bb);
            }
        }

        public void DrawingHoverRectangle(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Layer.DataHasChanged();
            }
        }

        public void DrawingRoute(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_addInfo == null)
            {
                var interactiveRoute = new InteractiveRoute();

                _addInfo = interactiveRoute.BeginDrawing(worldPosition);

                Layer.Add(_addInfo.Feature);
                Layer.AddRange(_addInfo.HelpFeatures);
                Layer.DataHasChanged();
            }
            else
            {
                var routeGeometry = (LineString)_addInfo.Feature.Geometry;

                if (routeGeometry.Vertices.Count > 1)
                {
                    // is end?
                    foreach (var item in routeGeometry.Vertices)
                    {
                        var p = viewport.WorldToScreen(item);

                        if (IsClick(p, screenPosition) == true)
                        {
                            EndDrawingRoute();
                            return;
                        }
                    }
                }

                _addInfo.Feature.Drawing(worldPosition);

                Layer.DataHasChanged();
            }
        }

        public void DrawingHoverRoute(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);
                Layer.DataHasChanged();
            }
        }

        private void EndDrawingRoute()
        {
            if (_addInfo == null)
            {
                return;
            }

            // TODO: need tested
            foreach (var item in _addInfo.HelpFeatures)
            {
                Layer.TryRemove(item);
            }

            _addInfo.Feature.EndDrawing();

            _addInfo = null;
        }

        public (bool, BoundingBox) DrawingPolygon(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_addInfo == null)
            {
                var interactivePolygon = new InteractivePolygon();

                _addInfo = interactivePolygon.BeginDrawing(worldPosition);

                Layer.Add(_addInfo.Feature);
                Layer.AddRange(_addInfo.HelpFeatures);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                var polygonGeometry = (LineString)_addInfo.Feature.Geometry;

                if (polygonGeometry.Vertices.Count > 2)
                {
                    // is end?

                    var p0 = viewport.WorldToScreen(polygonGeometry.Vertices[0]);

                    bool click = IsClick(p0, screenPosition);

                    if (click == true)
                    {
                        return EndDrawingPolygon();                    
                    }
                }

                _addInfo.Feature.Drawing(worldPosition);

                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
        }

        public void DrawingHoverPolygon(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);
                Layer.DataHasChanged();
            }
        }

        private (bool, BoundingBox) EndDrawingPolygon()
        {
            if (_addInfo == null)
            {
                return (false, new BoundingBox());
            }

            // TODO: need tested
            foreach (var item in _addInfo.HelpFeatures)
            {
                Layer.TryRemove(item);
            }

            _addInfo.Feature.EndDrawing();

            var bb = _addInfo.Feature.Geometry.BoundingBox;

            _addInfo = null;

            return (true, bb);
        }

        public (bool, BoundingBox) DrawingCircle(Point worldPosition)
        {
            if (_addInfo == null)
            {
                var interactiveCircle = new InteractiveCircle();

                _addInfo = interactiveCircle.BeginDrawing(worldPosition);

                Layer.Add(_addInfo.Feature);
                Layer.DataHasChanged();

                return (false, new BoundingBox());
            }
            else
            {
                _addInfo.Feature.EndDrawing();

                var bb = _addInfo.Feature.Geometry.BoundingBox;

                _addInfo = null;

                Layer.DataHasChanged();

                return (true, bb);
            }
        }

        public void DrawingHoverCircle(Point worldPosition)
        {
            if (_addInfo != null)
            {
                _addInfo.Feature.DrawingHover(worldPosition);

                Layer.DataHasChanged();
            }
        }



        public bool StartDragging(MapInfo mapInfo, double screenDistance)
        {
            if (mapInfo.Feature != null && mapInfo.Feature is InteractiveFeature interactiveFeature)
            {
                var vertexTouched = FindVertexTouched(mapInfo, interactiveFeature.EditVertices(), screenDistance);
                if (vertexTouched != null)
                {
                    _dragInfo.Feature = interactiveFeature;
                    _dragInfo.Vertex = vertexTouched;
                    _dragInfo.StartOffsetToVertex = mapInfo.WorldPosition - _dragInfo.Vertex;

                    return true; // to indicate start of drag
                }
            }

            return false;
        }

        public bool Dragging(Point worldPosition)
        {
            if (_dragInfo.Feature == null)
                return false;

            SetPointXY(_dragInfo.Vertex, worldPosition - _dragInfo.StartOffsetToVertex);

            if (_dragInfo.Feature.Geometry is Polygon polygon) // Not this only works correctly it the feature is in the outerring.
            {
                var count = polygon.ExteriorRing.Vertices.Count;
                var vertices = polygon.ExteriorRing.Vertices;
                var index = vertices.IndexOf(_dragInfo.Vertex);
                if (index >= 0)
                {
                    // It is a ring where the first should be the same as the last.
                    // So if the first was removed than set the last to the value of the new first
                    if (index == 0)
                    {
                        SetPointXY(vertices[count - 1], vertices[0]);
                    }
                    // If the last was removed then set the first to the value of the new last
                    else if (index == vertices.Count)
                    {
                        SetPointXY(vertices[0], vertices[count - 1]);
                    }
                }
            }

            _dragInfo.Feature.RenderedGeometry.Clear();
            Layer.DataHasChanged();
            return true;
        }

        public void StopDragging()
        {
            if (_dragInfo.Feature != null)
            {
                _dragInfo.Feature = null;
            }
        }

        private Point FindVertexTouched(MapInfo mapInfo, IEnumerable<Point> vertices, double screenDistance)
        {
            return vertices.OrderBy(v => v.Distance(mapInfo.WorldPosition)).FirstOrDefault(v => v.Distance(mapInfo.WorldPosition) < mapInfo.Resolution * screenDistance);
        }

        private void SetPointXY(Point target, Point position)
        {
            target.X = position.X;
            target.Y = position.Y;
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

    internal class EditManager_old
    {
        //public void DrawingRectangle(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        //{
        //    if (_addInfo.Feature == null)
        //    {
        //        var p0 = worldPosition.Clone();
        //        var p1 = worldPosition.Clone();
        //        var p2 = worldPosition.Clone();
        //        var p3 = worldPosition.Clone();

        //        _addInfo.Vertex = p2;
        //        _addInfo.Feature = FeatureBuilder.CreatePolygon(FeatureType.AOIRectangleDrawing.ToString(), new[] { p0, p1, p2, p3/*, p0*/ });
        //        _addInfo.Vertices = _addInfo.Feature.Geometry.MainVertices();

        //        Layer.Add(_addInfo.Feature);
        //        Layer.DataHasChanged();
        //    }
        //    else
        //    {
        //        var vertices = ((Polygon)_addInfo.Feature.Geometry).ExteriorRing.Vertices;
        //        var polygon = FeatureBuilder.CreatePolygon(FeatureType.AOIRectangle.ToString(), vertices);

        //        // TODO: need tested            
        //        Layer.TryRemove(_addInfo.Feature);

        //        Layer.Add(polygon);

        //        _addInfo.Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
        //        _addInfo.Feature = null;
        //        _addInfo.Vertex = null;

        //        Layer.DataHasChanged();
        //    }
        //}

        //public void DrawingHoverRectangle(Point worldPosition)
        //{
        //    if (_addInfo.Vertex != null)
        //    {
        //        var p2 = worldPosition.Clone();
        //        var rectangle = (Polygon)_addInfo.Feature.Geometry;
        //        var p0 = rectangle.ExteriorRing.Vertices[0];

        //        var p1 = new Point(p2.X, p0.Y);
        //        var p3 = new Point(p0.X, p2.Y);

        //        _addInfo.Vertex = p2;

        //        ((Polygon)_addInfo.Feature.Geometry).ExteriorRing.Vertices[0] = p0;
        //        ((Polygon)_addInfo.Feature.Geometry).ExteriorRing.Vertices[1] = p1;
        //        ((Polygon)_addInfo.Feature.Geometry).ExteriorRing.Vertices[2] = p2;
        //        ((Polygon)_addInfo.Feature.Geometry).ExteriorRing.Vertices[3] = p3;

        //        _addInfo.Vertices = _addInfo.Feature.Geometry.MainVertices();

        //        _addInfo.Feature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}

        //public void DrawingRoute(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        //{
        //    if (_addRouteInfo.Feature == null)
        //    {
        //        var firstPoint = worldPosition.Clone();
        //        // Add a second point right away. The second one will be the 'hover' vertex
        //        var secondPoint = worldPosition.Clone();

        //        _addRouteInfo.HoverVertex = secondPoint;
        //        _addRouteInfo.Feature = FeatureBuilder.CreateLineString(FeatureType.Route.ToString(), new[] { firstPoint });
        //        _addRouteInfo.EditFeature = FeatureBuilder.CreateLineString(FeatureType.RouteDrawing.ToString(), new[] { firstPoint, secondPoint });
        //        _addRouteInfo.Vertices = _addRouteInfo.Feature.Geometry.MainVertices();

        //        Layer.Add(_addRouteInfo.Feature);
        //        Layer.Add(_addRouteInfo.EditFeature);
        //        Layer.DataHasChanged();
        //    }
        //    else
        //    {
        //        var routeGeometry = (LineString)_addRouteInfo.Feature.Geometry;
        //        var editRouteGeometry = (LineString)_addRouteInfo.EditFeature.Geometry;

        //        if (routeGeometry.Vertices.Count > 1)
        //        {
        //            // is end?
        //            foreach (var item in routeGeometry.Vertices)
        //            {
        //                var p0 = viewport.WorldToScreen(item);

        //                bool click = IsClick(p0, screenPosition);

        //                //bool click = IsClick(item, worldPosition);

        //                if (click == true)
        //                {
        //                    EndDrawingRoute();
        //                    return;
        //                }
        //            }
        //        }

        //        // Set the final position of the 'hover' vertex (that was already part of the geometry)
        //        SetPointXY(_addRouteInfo.HoverVertex, worldPosition.Clone());

        //        var lastHoverVertex = _addRouteInfo.HoverVertex;
        //        var newHoverVertex = worldPosition.Clone(); // and create a new hover vertex

        //        _addRouteInfo.HoverVertex = newHoverVertex;

        //        routeGeometry.Vertices.Add(lastHoverVertex); // and add it to the geometry
        //        editRouteGeometry.Vertices = new[] { lastHoverVertex, newHoverVertex };

        //        _addRouteInfo.Feature.RenderedGeometry?.Clear();
        //        _addRouteInfo.EditFeature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}

        //public void DrawingHoverRoute(Point worldPosition)
        //{
        //    if (_addRouteInfo.HoverVertex != null)
        //    {
        //        SetPointXY(_addRouteInfo.HoverVertex, worldPosition);
        //        _addRouteInfo.EditFeature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}

        //private void EndDrawingRoute()
        //{
        //    if (_addRouteInfo.Feature == null)
        //    {
        //        return;
        //    }

        //    // TODO: need tested
        //    Layer.TryRemove(_addRouteInfo.EditFeature);

        //    //_addRouteInfo.Vertices.RemoveAt(_addRouteInfo.Vertices.Count - 1); // correct for double click
        //    _addRouteInfo.Feature = null;
        //    _addRouteInfo.EditFeature = null;
        //    _addRouteInfo.HoverVertex = null;
        //}

        //public void DrawingPolygon(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        //{
        //    if (_addPolygonInfo.Feature == null)
        //    {
        //        var firstPoint = worldPosition.Clone();
        //        // Add a second point right away. The second one will be the 'hover' vertex
        //        var secondPoint = worldPosition.Clone();

        //        _addPolygonInfo.HoverVertex = secondPoint;
        //        _addPolygonInfo.FeatureArea = FeatureBuilder.CreatePolygon(FeatureType.AOIPolygonAreaDrawing.ToString(), new[] { firstPoint });
        //        _addPolygonInfo.Feature = FeatureBuilder.CreateLineString(FeatureType.AOIPolygonBorderDrawing.ToString(), new[] { firstPoint });
        //        _addPolygonInfo.EditFeature = FeatureBuilder.CreateLineString(FeatureType.AOIPolygonDrawing.ToString(), new[] { firstPoint, secondPoint });
        //        _addPolygonInfo.Vertices = _addPolygonInfo.Feature.Geometry.MainVertices();

        //        Layer.Add(_addPolygonInfo.FeatureArea);
        //        Layer.Add(_addPolygonInfo.Feature);
        //        Layer.Add(_addPolygonInfo.EditFeature);
        //        Layer.DataHasChanged();
        //    }
        //    else
        //    {
        //        var polygonAreaGeometry = (Polygon)_addPolygonInfo.FeatureArea.Geometry;
        //        var polygonGeometry = (LineString)_addPolygonInfo.Feature.Geometry;
        //        var editPolygonGeometry = (LineString)_addPolygonInfo.EditFeature.Geometry;

        //        if (polygonGeometry.Vertices.Count > 2)
        //        {
        //            // is end?

        //            var p0 = viewport.WorldToScreen(polygonGeometry.Vertices[0]);

        //            bool click = IsClick(p0, screenPosition);

        //            if (click == true)
        //            {
        //                EndDrawingPolygon();
        //                return;
        //            }
        //        }

        //        // Set the final position of the 'hover' vertex (that was already part of the geometry)
        //        SetPointXY(_addPolygonInfo.HoverVertex, worldPosition.Clone());

        //        var lastHoverVertex = _addPolygonInfo.HoverVertex;
        //        var newHoverVertex = worldPosition.Clone(); // and create a new hover vertex

        //        _addPolygonInfo.HoverVertex = newHoverVertex;

        //        polygonAreaGeometry.ExteriorRing.Vertices.Add(lastHoverVertex);
        //        polygonGeometry.Vertices.Add(lastHoverVertex); // and add it to the geometry
        //        editPolygonGeometry.Vertices = new[] { lastHoverVertex, newHoverVertex };

        //        _addPolygonInfo.FeatureArea.RenderedGeometry?.Clear();
        //        _addPolygonInfo.Feature.RenderedGeometry?.Clear();
        //        _addPolygonInfo.EditFeature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}

        //public void DrawingHoverPolygon(Point worldPosition)
        //{
        //    if (_addPolygonInfo.HoverVertex != null)
        //    {
        //        SetPointXY(_addPolygonInfo.HoverVertex, worldPosition);
        //        _addPolygonInfo.EditFeature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}

        //private void EndDrawingPolygon()
        //{
        //    if (_addPolygonInfo.Feature == null)
        //    {
        //        return;
        //    }

        //    var vertices = ((LineString)_addPolygonInfo.Feature.Geometry).Vertices;
        //    var polygon = FeatureBuilder.CreatePolygon(FeatureType.AOIPolygon.ToString(), vertices);


        //    // TODO: need tested
        //    Layer.TryRemove(_addPolygonInfo.FeatureArea);
        //    Layer.TryRemove(_addPolygonInfo.Feature);
        //    Layer.TryRemove(_addPolygonInfo.EditFeature);

        //    Layer.Add(polygon);

        //    //_addRouteInfo.Vertices.RemoveAt(_addRouteInfo.Vertices.Count - 1); // correct for double click
        //    _addPolygonInfo.FeatureArea = null;
        //    _addPolygonInfo.Feature = null;
        //    _addPolygonInfo.EditFeature = null;
        //    _addPolygonInfo.HoverVertex = null;
        //}

        //public void DrawingCircle(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        //{
        //    if (_addCircleInfo.Feature == null)
        //    {
        //        var p0 = worldPosition.Clone();

        //        _addCircleInfo.Center = p0;
        //        _addCircleInfo.Feature = FeatureBuilder.CreateCircle(FeatureType.AOICircleDrawing.ToString(), p0, 0.0, 3);

        //        Layer.Add(_addCircleInfo.Feature);
        //        Layer.DataHasChanged();
        //    }
        //    else
        //    {
        //        var vertices = ((Polygon)_addCircleInfo.Feature.Geometry).ExteriorRing.Vertices;
        //        var circle = FeatureBuilder.CreatePolygon(FeatureType.AOICircle.ToString(), vertices);

        //        // TODO: need tested            
        //        Layer.TryRemove(_addCircleInfo.Feature);

        //        Layer.Add(circle);

        //        _addCircleInfo.Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
        //        _addCircleInfo.Feature = null;
        //        _addCircleInfo.Center = null;
        //        _addCircleInfo.SizeNE = null;

        //        Layer.DataHasChanged();
        //    }
        //}

        //public void DrawingHoverCircle(Point worldPosition)
        //{
        //    if (_addCircleInfo.Feature != null)
        //    {
        //        var p1 = worldPosition.Clone();
        //        var p0 = _addCircleInfo.Center;

        //        var radius = p0.Distance(p1);

        //        _addCircleInfo.SizeNE = p1;

        //        var feature = FeatureBuilder.CreateCircle(FeatureType.AOICircleDrawing.ToString(), p0, radius, 180);

        //        _addCircleInfo.Feature.Geometry = feature.Geometry;

        //        _addCircleInfo.Feature.RenderedGeometry?.Clear();
        //        Layer.DataHasChanged();
        //    }
        //}
    }
}
