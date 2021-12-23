using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.FeatureBuilders
{
    public class CircleBuilder : BaseFeatureBuilder
    {
        private bool _skip;
        private int _counter;
        private bool _isCreating;
        private bool _isDrawing = false;
        protected Point? _center;

        public override void Starting(Point worldPosition)
        {
            _skip = false;
            _counter = 0;
        }

        public override void Moving(Point worldPosition)
        {
            if (_counter++ > 0)
            {
                _skip = true;
            }
        }

        public override void Ending(Point worldPosition, Predicate<Point> isEnd)
        {
            if (_skip == false)
            {
                CreatingFeature(worldPosition);
            }
        }

        public override void Hover(Point worldPosition)
        {
            HoverCreatingFeature(worldPosition);
        }

        public void CreatingFeature(Point worldPosition)
        {
            CreatingFeature(worldPosition, point => true);
        }

        private bool _firstClick = true;

        public void CreatingFeature(Point worldPosition, Predicate<Point> isEnd)
        {
            if (_firstClick == true)
            {
                BeginDrawing(worldPosition);

                _isCreating = true;

                _firstClick = false;

                return;
            }
            else
            {
                EndDrawing();

                _isCreating = false;

                _firstClick = true;

                CreateCallback();

                return;
            }
        }

        public void HoverCreatingFeature(Point worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                Invalidate();
            }
        }

        public void BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                _center = worldPosition.Clone();

                var vertices = GetCircle(_center, 0.0, 3);

                var geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                Feature = new Feature() { Geometry = geometry };
                ExtraFeatures = new List<IFeature>();
            }
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

        public void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true && _center != null)
            {
                var geometry = Feature.Geometry;

                var p1 = worldPosition.Clone();

                var radius = _center.Distance(p1);

                var vertices = GetCircle(_center, radius, 180);

                ((Polygon)geometry).ExteriorRing = new LinearRing(vertices);

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
