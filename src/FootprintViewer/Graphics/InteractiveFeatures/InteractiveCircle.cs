using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.UI;
using NetTopologySuite.Triangulate.QuadEdge;
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

        private bool _isDragging = false;
        private Point _vertex;
        private Point _startOffsetToVertex;

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
            if (Geometry != null && _isDragging == true)
            {
                return new List<Point>() { _center };
            }
            if (Geometry != null && _isDrawing == false)
            {
                return new List<Point>() { _center, _sizeNE };
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
            
            if (vertexTouched != null && vertexTouched == _sizeNE)
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

            var p1 = worldPosition - _startOffsetToVertex;

            var radius = _center.Distance(p1);

            var angleRad = 45.0 / 180.0 * Math.PI;

            _sizeNE = new Point(radius * Math.Sin(angleRad) + _center.X, radius * Math.Cos(angleRad) + _center.Y);

            var vertices = GetCircle(_center, radius, 180);

            Geometry = new Polygon()
            {
                ExteriorRing = new LinearRing(vertices)
            };

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
