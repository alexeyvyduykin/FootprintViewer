using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Designers
{
    public class RectangleDesigner : BaseDesigner
    {
        private bool _isDrawing = false;
        private bool _isCreating = false;

        private bool _skip;
        private int _counter;

        public RectangleDesigner() : base() { }

        public override IEnumerable<Point> GetActiveVertices() => new List<Point>();

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

        public override void Ending(Point worldPosition, Predicate<Point>? isEnd)
        {
            if (_skip == false)
            {
                CreatingFeature(worldPosition);
            }
        }

        public override void Hovering(Point worldPosition)
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

                var p0 = worldPosition.Clone();
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();
                var p3 = worldPosition.Clone();

                var geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0, p1, p2, p3 })
                };

                Feature = new Feature() { Geometry = geometry };
                ExtraFeatures = new List<IFeature>();
            }
        }

        public void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var geometry = Feature.Geometry;
                var p2 = worldPosition.Clone();
                var rectangle = (Polygon)geometry;
                var p0 = rectangle.ExteriorRing.Vertices[0];

                var p1 = new Point(p2.X, p0.Y);
                var p3 = new Point(p0.X, p2.Y);

                ((Polygon)geometry).ExteriorRing.Vertices[0] = p0;
                ((Polygon)geometry).ExteriorRing.Vertices[1] = p1;
                ((Polygon)geometry).ExteriorRing.Vertices[2] = p2;
                ((Polygon)geometry).ExteriorRing.Vertices[3] = p3;

                Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var geometry = Feature.Geometry;

                var vertices = ((Polygon)geometry).ExteriorRing.Vertices;

                Feature.Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };

                Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        //public IList<Point> EditVertices()
        //{
        //    if (Geometry != null && _isDrawing == false)
        //    {
        //        return ((Polygon)Geometry).ExteriorRing.Vertices;
        //    }

        //    return new List<Point>();
        //}
    }
}
