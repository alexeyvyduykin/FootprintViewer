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
