using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
{
    public class RotateDecorator : BaseDecorator
    {
        private readonly Point _center;
        private Point _rotateRight;
        private Point _startRotateRight;
        private Point _startOffsetToVertex;
        private IGeometry? _startGeometry;
        private double _halfDiagonal;
        // HACK: without this locker Moving() passing not his order
        private bool _isRotating = false;

        public RotateDecorator(IFeature featureSource) : base(featureSource)
        {
            _rotateRight = new Point(featureSource.Geometry.BoundingBox.Right, featureSource.Geometry.BoundingBox.Centroid.Y);

            _center = featureSource.Geometry.BoundingBox.Centroid;

            _startRotateRight = _rotateRight;

            _halfDiagonal = Diagonal(FeatureSource.Geometry.BoundingBox) / 2.0;

            _startOffsetToVertex = new Point();
        }

        public override void Ending()
        {
            _rotateRight = new Point(FeatureSource.Geometry.BoundingBox.Right, FeatureSource.Geometry.BoundingBox.Centroid.Y);

            _isRotating = false;
        }

        public override IEnumerable<Point> GetActiveVertices() => new[] { _rotateRight };

        public override void Moving(Point worldPosition)
        {
            if (_isRotating == true && _startGeometry != null)
            {
                var p1 = worldPosition - _startOffsetToVertex;

                var distance = _startRotateRight.Distance(p1);

                var sign = (p1 - _startRotateRight).Y >= 0 ? -1 : 1;

                var geometry = Copy(_startGeometry);

                var degrees = sign * (distance * 360.0 / _halfDiagonal);

                Geomorpher.Rotate(geometry, degrees, _center);

                _rotateRight = new Point(_startRotateRight.X, p1.Y);

                FeatureSource.Geometry = geometry;

                FeatureSource.RenderedGeometry.Clear();
            }
        }

        public override void Starting(Point worldPosition)
        {
            _startRotateRight = _rotateRight;

            _startOffsetToVertex = worldPosition - _startRotateRight;

            _startGeometry = Copy(FeatureSource.Geometry);

            _halfDiagonal = Diagonal(FeatureSource.Geometry.BoundingBox) / 2.0;

            _isRotating = true;
        }

        private double Diagonal(BoundingBox boundingBox)
        {
            return Math.Sqrt(boundingBox.Width * boundingBox.Width + boundingBox.Height * boundingBox.Height);
        }
    }
}
