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
    internal class PolygonDesigner : BaseDesigner, IAreaDesigner
    {
        private bool _skip;
        private int _counter;
        private bool _isDrawing = false;

        private GeometryFeature? _extraLineString;
        private GeometryFeature? _extraPolygon;

        private List<Coordinate> _extraPolygonCoordinates = new();
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

                BeginCreatingCallback();

                _firstClick = false;

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
            var polygonGeometry = (LineString)Feature.Geometry!;

            if (polygonGeometry.Coordinates.Length > 2)
            {
                var click = isEnd?.Invoke(polygonGeometry.Coordinates[0].ToMPoint());

                if (click == true)
                {
                    return true;
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

            _extraLineString = new[] { p0, p1 }.ToLineString().ToFeature("ExtraPolygonHoverLine");
            _extraPolygonCoordinates = new() { p0 };
            _extraPolygon = _extraPolygonCoordinates.ToPolygon().ToFeature("ExtraPolygonArea");

            _featureCoordinates = new() { p0 };
            Feature = _featureCoordinates.ToLineString().ToFeature();
            ExtraFeatures = new[] { _extraLineString, _extraPolygon };
        }

        private void Drawing(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p0 = ((LineString)_extraLineString!.Geometry!).EndPoint;
                var p1 = worldPosition.ToCoordinate();
                var p2 = worldPosition.ToCoordinate();

                _extraLineString.Geometry = new[] { p1, p2 }.ToLineString();

                _extraPolygonCoordinates.Add(new Coordinate(p0.X, p0.Y));
                _extraPolygon!.Geometry = _extraPolygonCoordinates.ToPolygon();

                _featureCoordinates.Add(new Coordinate(p0.X, p0.Y));
                Feature.Geometry = _featureCoordinates.ToLineString();

                Feature.RenderedGeometry?.Clear();
                _extraLineString.RenderedGeometry?.Clear();
                _extraPolygon.RenderedGeometry?.Clear();
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

                Feature.Geometry = _featureCoordinates.ToPolygon();
            }
        }

        public double Area() => MathHelper.ComputeSphericalArea(_featureCoordinates.Select(s => SphericalMercator.ToLonLat(s.X, s.Y)));
    }
}
