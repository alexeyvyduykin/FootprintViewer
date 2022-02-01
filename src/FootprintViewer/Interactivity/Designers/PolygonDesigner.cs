using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Designers
{
    public class PolygonDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;

        private IFeature? _extraLineString;
        private IFeature? _extraPolygon;

        public override IEnumerable<Point> GetActiveVertices() => Feature.Geometry.MainVertices();

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
                CreatingFeature(worldPosition, isEnd);
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

        public void CreatingFeature(Point worldPosition, Predicate<Point>? isEnd)
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

        public void HoverCreatingFeature(Point worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                HoverCreatingCallback();

                Invalidate();
            }
        }

        public bool IsEndDrawing(Point worldPosition, Predicate<Point>? isEnd)
        {
            var polygonGeometry = (LineString)Feature.Geometry;

            if (polygonGeometry.Vertices.Count > 2)
            {
                var click = isEnd?.Invoke(polygonGeometry.Vertices[0]);

                if (click == true)
                {
                    return true;
                }
            }

            return false;
        }

        public void BeginDrawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                return;
            }

            _isDrawing = true;

            var p0 = worldPosition.Clone();
            // Add a second point right away. The second one will be the 'hover' vertex
            var p1 = worldPosition.Clone();

            var geometry = new LineString(new[] { p0 });

            _extraLineString = new Feature
            {
                Geometry = new LineString(new[] { p0, p1 }),
                ["Name"] = "ExtraPolygonHoverLine",
            };

            _extraPolygon = new Feature
            {
                Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(new[] { p0 })
                },
                ["Name"] = "ExtraPolygonArea",
            };

            Feature = new Feature() { Geometry = geometry };
            ExtraFeatures = new List<IFeature>() { _extraLineString, _extraPolygon };
        }

        public void Drawing(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString.Geometry).EndPoint;
                var p1 = worldPosition.Clone();
                var p2 = worldPosition.Clone();

                ((Polygon)_extraPolygon.Geometry).ExteriorRing.Vertices.Add(p0);
                ((LineString)Feature.Geometry).Vertices.Add(p0); // and add it to the geometry
                ((LineString)_extraLineString.Geometry).Vertices = new[] { p1, p2 };

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
                _extraPolygon.RenderedGeometry?.Clear();
            }
        }

        public void DrawingHover(Point worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_extraLineString.Geometry).EndPoint.X = worldPosition.X;
                ((LineString)_extraLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _extraLineString.RenderedGeometry?.Clear();
            }
        }

        public void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;

                var vertices = ((LineString)Feature.Geometry).Vertices;

                Feature.Geometry = new Polygon()
                {
                    ExteriorRing = new LinearRing(vertices)
                };
            }
        }

    }
}
