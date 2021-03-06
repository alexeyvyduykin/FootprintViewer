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
    internal class RectangleDesigner : BaseDesigner, IAreaDesigner
    {
        private bool _isDrawing = false;
        private bool _skip;
        private int _counter;
        private List<Coordinate> _featureCoordinates = new();

        public RectangleDesigner() : base() { }

        public override IEnumerable<MPoint> GetActiveVertices() => Array.Empty<MPoint>();

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

        //public void CreatingFeature(MPoint worldPosition)
        //{
        //    CreatingFeature(worldPosition, point => true);
        //}

        private bool _firstClick = true;

        private void CreatingFeature(MPoint worldPosition/*, Predicate<MPoint> isEnd*/)
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

        private void HoverCreatingFeature(MPoint worldPosition)
        {
            if (_firstClick == false)
            {
                DrawingHover(worldPosition);

                HoverCreatingCallback();

                Invalidate();
            }
        }

        private void BeginDrawing(MPoint worldPosition)
        {
            if (_isDrawing == false)
            {
                _isDrawing = true;

                var p0 = worldPosition.ToCoordinate();
                var p1 = worldPosition.ToCoordinate();
                var p2 = worldPosition.ToCoordinate();
                var p3 = worldPosition.ToCoordinate();

                _featureCoordinates = new() { p0, p1, p2, p3 };
                Feature = _featureCoordinates.ToPolygon().ToFeature();
                ExtraFeatures = new List<GeometryFeature>();
            }
        }

        private void DrawingHover(MPoint worldPosition)
        {
            if (_isDrawing == true)
            {
                var p2 = worldPosition.ToCoordinate();
                var p0 = _featureCoordinates[0];
                var p1 = (p2.X, p0.Y).ToCoordinate();
                var p3 = (p0.X, p2.Y).ToCoordinate();

                _featureCoordinates = new() { p0, p1, p2, p3 };
                Feature.Geometry = _featureCoordinates.ToPolygon();

                Feature.RenderedGeometry?.Clear(); // You need to clear the cache to see changes.
            }
        }

        private void EndDrawing()
        {
            if (_isDrawing == true)
            {
                _isDrawing = false;
            }
        }

        public double Area() => MathHelper.ComputeSphericalArea(_featureCoordinates.Select(s => SphericalMercator.ToLonLat(s.X, s.Y)));
    }
}
