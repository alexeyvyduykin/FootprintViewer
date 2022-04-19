using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Designers
{
    public class RectangleDesigner : BaseDesigner
    {
        private bool _isDrawing = false;
        private bool _skip;
        private int _counter;

        public RectangleDesigner() : base() { }

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

                BeginCreatingCallback();

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

                var p0 = worldPosition.Copy().ToCoordinate();
                var p1 = worldPosition.Copy().ToCoordinate();
                var p2 = worldPosition.Copy().ToCoordinate();
                var p3 = worldPosition.Copy().ToCoordinate();

//                var geometry = new Polygon()
//                {
//                    ExteriorRing = new LinearRing(new[] { p0, p1, p2, p3 })
//};

                var geometry = new GeometryFactory().CreatePolygon(new[] { p0, p1, p2, p3 });

                Feature = new GeometryFeature() { Geometry = geometry };
                ExtraFeatures = new List<GeometryFeature>();
            }
        }

        public void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var geometry = Feature.Geometry;
                var p2 = worldPosition.Copy().ToCoordinate();
                var rectangle = (Polygon)geometry;
                var p0 = rectangle.ExteriorRing.Coordinates[0];

                var p1 = new MPoint(p2.X, p0.Y).ToCoordinate();
                var p3 = new MPoint(p0.X, p2.Y).ToCoordinate();

   //             ((Polygon)geometry).ExteriorRing.Vertices[0] = p0;
   //             ((Polygon)geometry).ExteriorRing.Vertices[1] = p1;
   //             ((Polygon)geometry).ExteriorRing.Vertices[2] = p2;
   //             ((Polygon)geometry).ExteriorRing.Vertices[3] = p3;

                var poly = new GeometryFactory().CreatePolygon(new[] { p0, p1, p2, p3 });
                Feature.Geometry = poly;

                Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        public void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var geometry = Feature.Geometry;

                var vertices = ((Polygon)geometry).ExteriorRing.Coordinates;
               
                var poly = new GeometryFactory().CreatePolygon(vertices);

                Feature.Geometry = poly/*new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                }*/;
            
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
