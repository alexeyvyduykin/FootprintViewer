using Mapsui.Nts;
using Mapsui;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;

namespace FootprintViewer.Interactivity.Designers
{
    public class CircleDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;
        protected MPoint? _center;

        public override IEnumerable<MPoint> GetActiveVertices() => new MPoint[] { };

        public override void Starting(MPoint worldPosition)
        {
            _skip = false;
            _counter = 0;
        }

        public override void Moving(MPoint worldPosition)
        {
            if (_counter++ > 0)
            {
                _skip = true;
            }
        }

        public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            if (_skip == false)
            {
                CreatingFeature(worldPosition);
            }
        }

        public override void Hovering(MPoint worldPosition)
        {
            HoverCreatingFeature(worldPosition);
        }

        public void CreatingFeature(MPoint worldPosition)
        {
            CreatingFeature(worldPosition, point => true);
        }

        private bool _firstClick = true;

        public void CreatingFeature(MPoint worldPosition, Predicate<MPoint> isEnd)
        {
            if (_firstClick == true)
            {
                BeginDrawing(worldPosition);

                _firstClick = false;

                return;
            }
            else
            {
                EndDrawing();

                _firstClick = true;

                EndCreatingCallback();

                return;
            }
        }

        public void HoverCreatingFeature(MPoint worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                HoverCreatingCallback();

                Invalidate();
            }
        }

        public void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                _center = worldPosition.Copy();

                var vertices = GetCircle(_center, 0.0, 3).ToArray();

                //var geometry = new Polygon()
                //{
                //    ExteriorRing = new LinearRing(vertices)
                //};

                var geometry = new GeometryFactory().CreatePolygon(vertices);

                Feature = new GeometryFeature() { Geometry = geometry };
                ExtraFeatures = new List<GeometryFeature>();
            }
        }

        private IList<Coordinate> GetCircle(MPoint center, double radius, double quality)
        {
            var centerX = center.X;
            var centerY = center.Y;

            //var radius = Radius.Meters / Math.Cos(Center.Latitude / 180.0 * Math.PI);
            var increment = 360.0 / (quality < 3.0 ? 3.0 : (quality > 360.0 ? 360.0 : quality));
            var vertices = new List<Coordinate>();

            for (double angle = 0; angle < 360; angle += increment)
            {
                var angleRad = angle / 180.0 * Math.PI;
                vertices.Add(new Coordinate(radius * Math.Sin(angleRad) + centerX, radius * Math.Cos(angleRad) + centerY));
            }

            return vertices;
        }

        public void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true && _center != null)
            {
                var geometry = Feature.Geometry;

                var p1 = worldPosition.Copy();

                var radius = _center.Distance(p1);

                var vertices = GetCircle(_center, radius, 180).ToArray();

                //((Polygon)geometry).ExteriorRing = new LinearRing(vertices);

                geometry = new GeometryFactory().CreatePolygon(vertices);
                Feature.Geometry = geometry;

                Feature.RenderedGeometry?.Clear();
            }
        }

        public void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }
    }
}
