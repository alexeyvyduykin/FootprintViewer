using DynamicData;
using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Interactivity.Designers
{
    public class PolygonDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;

        private GeometryFeature? _extraLineString;
        private GeometryFeature? _extraPolygon;

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            if (Feature.Geometry != null)
            {
                return Feature.Geometry.MainVertices().Select(s=>s.ToMPoint());
            }

            return new MPoint[] { };
        }

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
                CreatingFeature(worldPosition, isEnd);
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

        public void CreatingFeature(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            if (_firstClick == true)
            {
                BeginDrawing(worldPosition);

                BeginCreatingCallback();

                _firstClick = false;

                return;
            }
            else
            {
                var res = IsEndDrawing(worldPosition, isEnd);

                if (res == true)
                {
                    EndDrawing();

                    _firstClick = true;

                    EndCreatingCallback();

                    return;
                }
                else
                {
                    Drawing(worldPosition);

                    CreatingCallback();

                    return;
                }
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

        public bool IsEndDrawing(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            var polygonGeometry = (LineString)Feature.Geometry;

            if (polygonGeometry.Coordinates.Count() > 2)
            {
                var click = isEnd?.Invoke(polygonGeometry.Coordinates[0].ToMPoint());

                if (click == true)
                {
                    return true;
                }
            }

            return false;
        }

        public void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                return;
            }

            _isDrawing = true;

            var p0 = worldPosition.Copy().ToCoordinate();
            // Add a second point right away. The second one will be the 'hover' vertex
            var p1 = worldPosition.Copy().ToCoordinate();

            var geometry = new LineString(new[] { p0 });

            _extraLineString = new GeometryFeature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = "ExtraPolygonHoverLine",
};

            var poly = new GeometryFactory().CreatePolygon(new[] { p0 });

            _extraPolygon = new GeometryFeature
            {
                Geometry = poly/*new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0 })
                }*/,
                ["Name"] = "ExtraPolygonArea",
            };

            Feature = new GeometryFeature() { Geometry = geometry };
            ExtraFeatures = new List<GeometryFeature>() { _extraLineString, _extraPolygon };
        }

        public void Drawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString!.Geometry).EndPoint;
                var p1 = worldPosition.Copy().ToCoordinate();
                var p2 = worldPosition.Copy().ToCoordinate();

                ((Polygon)_extraPolygon!.Geometry).ExteriorRing.Coordinates/*Vertices*/.Add(new[] { new Coordinate(p0.X, p0.Y) });
                ((LineString)Feature.Geometry).Coordinates.Add(new[] { new Coordinate(p0.X, p0.Y) }); // and add it to the geometry
                //((LineString)_extraLineString.Geometry).Vertices = new[] { p1, p2 };
                _extraLineString.Geometry = new LineString(new[] { p1, p2 });

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
                _extraPolygon.RenderedGeometry?.Clear();
            }
        }

        public void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_extraLineString!.Geometry).EndPoint.X = worldPosition.X;
                ((LineString)_extraLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _extraLineString.RenderedGeometry?.Clear();
            }
        }

        public void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var vertices = ((LineString)Feature.Geometry).Coordinates;//Vertices;

                var poly = new GeometryFactory().CreatePolygon(vertices);

                Feature.Geometry = poly/*new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                }*/;
            }
        }

    }
}
