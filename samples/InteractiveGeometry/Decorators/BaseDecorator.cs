using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace InteractiveGeometry
{
    public abstract class BaseDecorator : BaseInteractiveObject, IDecorator
    {
        private readonly GeometryFeature _featureSource;

        public BaseDecorator(GeometryFeature featureSource)
        {
            _featureSource = featureSource;
        }

        protected void UpdateGeometry(Geometry geometry)
        {
            _featureSource.Geometry = geometry;

            _featureSource.RenderedGeometry.Clear();

            Invalidate();
        }

        public GeometryFeature FeatureSource => _featureSource;
    }
}
