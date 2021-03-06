using InteractiveGeometry.Helpers;
using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InteractiveGeometry
{
    internal class RouteDesigner : BaseDesigner, IRouteDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;
        private GeometryFeature? _extraLineString;
        private List<Coordinate> _featureCoordinates = new();

        public override IEnumerable<MPoint> GetActiveVertices()
        {
            if (Feature.Geometry != null)
            {
                return Feature.Geometry.MainVertices().Select(s => s.ToMPoint());
            }

            return Array.Empty<MPoint>();
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

        //private void CreatingFeature(MPoint worldPosition)
        //{
        //    CreatingFeature(worldPosition, point => true);
        //}

        private bool _firstClick = true;

        private void CreatingFeature(MPoint worldPosition, Predicate<MPoint>? isEnd)
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
                var res = IsEndDrawing(/*worldPosition,*/ isEnd);

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

        private void HoverCreatingFeature(MPoint worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                HoverCreatingCallback();

                Invalidate();
            }
        }

        private bool IsEndDrawing(/*MPoint worldPosition,*/ Predicate<MPoint>? isEnd)
        {
            var routeGeometry = (LineString)Feature.Geometry!;

            if (routeGeometry.Coordinates.Length > 1)
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

        private void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                return;
            }

            _isDrawing = true;

            var p0 = worldPosition.ToCoordinate();
            var p1 = worldPosition.ToCoordinate();

            _extraLineString = new[] { p0, p1 }.ToLineString().ToFeature("ExtraRouteHoverLine");

            _featureCoordinates = new() { p0 };
            Feature = _featureCoordinates.ToLineString().ToFeature();
            ExtraFeatures = new List<GeometryFeature>() { _extraLineString };
        }

        private void Drawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString!.Geometry!).EndPoint.ToMPoint().ToCoordinate();
                var p1 = worldPosition.ToCoordinate();
                var p2 = worldPosition.ToCoordinate();

                _featureCoordinates.Add(p0);
                Feature.Geometry = _featureCoordinates.ToLineString();

                _extraLineString.Geometry = new[] { p1, p2 }.ToLineString();

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
            }
        }

        private void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                ((LineString)_extraLineString!.Geometry!).EndPoint.X = worldPosition.X;
                ((LineString)_extraLineString.Geometry).EndPoint.Y = worldPosition.Y;

                _extraLineString.RenderedGeometry?.Clear();
            }
        }

        private void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;
            }
        }

        public double Distance()
        {
            if (Feature.Geometry != null)
            {
                var verts0 = Feature.Geometry.Coordinates;
                var verts1 = ExtraFeatures.Single().Geometry!.Coordinates;
                var verts = verts0.Union(verts1);
                var vertices = verts.Select(s => SphericalMercator.ToLonLat(s.X, s.Y));
                return MathHelper.ComputeSphericalDistance(vertices);
            }

            return 0;
        }
    }
}
