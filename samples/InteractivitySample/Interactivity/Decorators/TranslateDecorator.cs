using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
{
    public class TranslateDecorator : BaseDecorator
    {
        private Point _center;
        private Point _startCenter;
        private Point _startOffsetToVertex;
        private IGeometry? _startGeometry;
        // HACK: without this locker Moving() passing not his order
        private bool _isTranslating = false;

        public TranslateDecorator(IFeature featureSource) : base(featureSource)
        {
            _center = featureSource.Geometry.BoundingBox.Centroid;

            _startCenter = _center;

            _startOffsetToVertex = new Point();
        }

        public override void Ending()
        {
            _isTranslating = false;
        }

        public override IEnumerable<Point> GetActiveVertices() => new[] { _center };

        public override void Moving(Point worldPosition)
        {
            if (_isTranslating == true && _startGeometry != null)
            {
                var p1 = worldPosition - _startOffsetToVertex;

                var delta = p1 - _startCenter;

                var geometry = Copy(_startGeometry);

                Geomorpher.Translate(geometry, delta.X, delta.Y);

                _center = geometry.BoundingBox.Centroid;

                FeatureSource.Geometry = geometry;

                FeatureSource.RenderedGeometry.Clear();
            }
        }

        public override void Starting(Point worldPosition)
        {
            _startCenter = _center;

            _startOffsetToVertex = worldPosition - _startCenter;

            _startGeometry = Copy(FeatureSource.Geometry);

            _isTranslating = true;
        }
    }
}
