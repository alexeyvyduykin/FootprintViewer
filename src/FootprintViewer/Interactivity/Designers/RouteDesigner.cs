using Mapsui.Nts;
using Mapsui;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Linq;
using Mapsui.Nts.Extensions;
using DynamicData;

namespace FootprintViewer.Interactivity.Designers
{
    public class RouteDesigner : BaseDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;
        private GeometryFeature? _extraLineString;

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            if (Feature.Geometry != null)
            {
                return Feature.Geometry.MainVertices().Select(s => s.ToMPoint());
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
            var routeGeometry = (LineString)Feature.Geometry;

            if (routeGeometry.Coordinates.Count() > 1)
            {
                foreach (var item in routeGeometry.Coordinates)
                {
                    var click = isEnd?.Invoke(item.ToMPoint());
                    if (click == true)
                    {
                        return true;
                    }
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
                ["Name"] = "ExtraRouteHoverLine",
            };

            Feature = new GeometryFeature() { Geometry = geometry };
            ExtraFeatures = new List<GeometryFeature>() { _extraLineString };
        }

        public void Drawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString!.Geometry).EndPoint.ToMPoint().ToCoordinate();
                var p1 = worldPosition.Copy().ToCoordinate();
                var p2 = worldPosition.Copy().ToCoordinate();

                ((LineString)Feature.Geometry).Coordinates.Add(new[] { p0 }); // and add it to the geometry
                //((LineString)_extraLineString.Geometry).Vertices = new[] { p1, p2 };
                _extraLineString.Geometry = new LineString(new[] { p1, p2 });

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
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
            }
        }
    }
}
