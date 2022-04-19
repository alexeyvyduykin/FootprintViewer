using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity.Decorators
{
    public class ScaleDecorator : BaseDecorator
    {
        private readonly MPoint _center;
        private MPoint _sizeNE;
        private MPoint _startSizeNE;
        private MPoint _startOffsetToVertex;
        private Geometry? _startGeometry;
        private double _startScale;
        // HACK: without this locker Moving() passing not his order
        private bool _isScaling = false;

        public ScaleDecorator(GeometryFeature featureSource) : base(featureSource)
        {
            _sizeNE = featureSource.Extent.TopRight;//Geometry.BoundingBox.TopRight;

            _center = featureSource.Extent.Centroid;//Geometry.BoundingBox.Centroid;

            _startSizeNE = _sizeNE;

            _startOffsetToVertex = new MPoint();
        }

        public override void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd)
        {
            _isScaling = false;
        }

        public override IEnumerable<MPoint> GetActiveVertices() => new[] { _sizeNE };

        public override void Moving(MPoint worldPosition)
        {
            if (_isScaling == true && _startGeometry != null)
            {
                var p1 = worldPosition - _startOffsetToVertex;

                var scale = _center.Distance(p1);

                var geometry = Copy(_startGeometry);

                Geomorpher.Scale(geometry, scale / _startScale, _center.ToPoint());

                _sizeNE = geometry.EnvelopeInternal.ToMRect().TopRight;// BoundingBox.TopRight;

                FeatureSource.Geometry = geometry;

                FeatureSource.RenderedGeometry.Clear();
            }
        }

        public override void Starting(MPoint worldPosition)
        {
            _startSizeNE = _sizeNE;

            _startOffsetToVertex = worldPosition - _startSizeNE;

            _startGeometry = Copy(FeatureSource.Geometry);

            _startScale = _center.Distance(_startSizeNE);

            _isScaling = true;
        }

        public override void Hovering(MPoint worldPosition)
        {

        }
    }
}
