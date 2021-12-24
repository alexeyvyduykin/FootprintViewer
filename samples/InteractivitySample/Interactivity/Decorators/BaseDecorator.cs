using Mapsui.Geometries;
using Mapsui.Providers;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity.Decorators
{
    public abstract class BaseDecorator : BaseInteractiveObject, IDecorator
    {
        private readonly IFeature _featureSource;

        public BaseDecorator(IFeature featureSource)
        {
            _featureSource = featureSource;
        }

        public abstract IEnumerable<Point> GetActiveVertices();

        public IFeature FeatureSource => _featureSource;
    }
}
