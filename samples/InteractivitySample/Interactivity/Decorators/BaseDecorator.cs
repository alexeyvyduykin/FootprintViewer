using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
{
    public abstract class BaseDecorator : IDecorator
    {
        private readonly IFeature _featureSource;

        public BaseDecorator(IFeature featureSource)
        {
            _featureSource = featureSource;
        }

        protected IGeometry Copy(IGeometry geometry)
        {
            var g = geometry.Copy();
            var count = g.MainVertices().Count;
            g.MainVertices().RemoveAt(count - 1);
            return g;
        }

        public abstract IEnumerable<Point> GetActiveVertices();

        public abstract void Starting(Point worldPosition);

        public abstract void Moving(Point worldPosition);

        public abstract void Ending();

        public IFeature FeatureSource => _featureSource;
    }
}
