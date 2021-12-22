using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Decorators
{
    public class ScaleDecorator : BaseDecorator
    {
        private readonly Point _center;
        private Point _sizeNE;
        private Point _startSizeNE;
        private Point _startOffsetToVertex;
        private IGeometry? _startGeometry;
        private double _startScale;
        // HACK: without this locker Moving() passing not his order
        private bool _isScaling = false;

        public ScaleDecorator(IFeature featureSource) : base(featureSource)
        {
            _sizeNE = featureSource.Geometry.BoundingBox.TopRight;

            _center = featureSource.Geometry.BoundingBox.Centroid;

            _startSizeNE = _sizeNE;

            _startOffsetToVertex = new Point();
        }

        public override void Ending()
        {
            _isScaling = false;
        }

        public override IEnumerable<Point> GetActiveVertices() => new[] { _sizeNE };

        public override void Moving(Point worldPosition)
        {
            if (_isScaling == true && _startGeometry != null)
            {
                var p1 = worldPosition - _startOffsetToVertex;

                var scale = _center.Distance(p1);

                var geometry = Copy(_startGeometry);

                Geomorpher.Scale(geometry, scale / _startScale, _center);

                _sizeNE = geometry.BoundingBox.TopRight;

                FeatureSource.Geometry = geometry;

                FeatureSource.RenderedGeometry.Clear();
            }
        }

        public override void Starting(Point worldPosition)
        {
            _startSizeNE = _sizeNE;

            _startOffsetToVertex = worldPosition - _startSizeNE;

            _startGeometry = Copy(FeatureSource.Geometry);

            _startScale = _center.Distance(_startSizeNE);

            _isScaling = true;
        }
    }
}
