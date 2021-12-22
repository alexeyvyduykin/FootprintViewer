using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.Decorators
{
    public abstract class BaseDecorator : IDecorator
    {
        private readonly IFeature _featureSource;

        public BaseDecorator(IFeature featureSource)
        {
            _featureSource = featureSource;
        }

        public abstract IEnumerable<Point> GetActiveVertices();

        public abstract void Starting(Point worldPosition);

        public abstract void Moving(Point worldPosition);

        public abstract void Ending();

        public IFeature FeatureSource => _featureSource;
    }
}
