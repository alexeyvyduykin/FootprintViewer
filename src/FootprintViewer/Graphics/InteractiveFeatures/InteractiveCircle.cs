using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
//using NetTopologySuite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer
{
    public class InteractiveCircle : InteractiveFeature
    {
        private bool _isDrawing = false;
        private Point _center;
        private Point _sizeNE;

        public override AddInfo BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                _center = worldPosition.Clone();
                _sizeNE = _center;

                var vertices = GetCircle(_center, 0.0, 3);

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                this["Name"] = FeatureType.AOICircleDrawing.ToString();
            }

            return new AddInfo()
            {
                Feature = this,
                HelpFeatures = new List<IFeature>(),
            };
        }

        private IList<Point> GetCircle(Point center, double radius, double quality)
        {
            var centerX = center.X;
            var centerY = center.Y;

            //var radius = Radius.Meters / Math.Cos(Center.Latitude / 180.0 * Math.PI);
            var increment = 360.0 / (quality < 3.0 ? 3.0 : (quality > 360.0 ? 360.0 : quality));
            var vertices = new List<Point>();

            for (double angle = 0; angle < 360; angle += increment)
            {
                var angleRad = angle / 180.0 * Math.PI;
                vertices.Add(new Point(radius * Math.Sin(angleRad) + centerX, radius * Math.Cos(angleRad) + centerY));
            }

            return vertices;
        }

        public override void Drawing(Point worldPosition)
        {
            EndDrawing();
        }

        public override void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p1 = worldPosition.Clone();

                var radius = _center.Distance(p1);

                var angleRad = 45.0 / 180.0 * Math.PI;

                _sizeNE = new Point(radius * Math.Sin(angleRad) + _center.X, radius * Math.Cos(angleRad) + _center.Y);

                var vertices = GetCircle(_center, radius, 180);

                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                RenderedGeometry?.Clear();
            }
        }

        public override void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                this["Name"] = FeatureType.AOICircle.ToString();

                RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null && _isDrawing == false)
            {
                return new List<Point>() { _center, _sizeNE };
            }

            return new List<Point>();
        }

        public override bool BeginDragging(Point worldPosition, double screenDistance)
        {
            throw new NotImplementedException();
        }

        public override bool Dragging(Point worldPosition)
        {
            throw new NotImplementedException();
        }

        public override void EndDragging()
        {
            throw new NotImplementedException();
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
