using Mapsui.Providers;

namespace InteractiveGeometry
{
    public abstract class BaseDecorator : BaseInteractiveObject, IDecorator
    {
        private readonly IFeature _featureSource;

        public BaseDecorator(IFeature featureSource)
        {
            _featureSource = featureSource;
        }

        public IFeature FeatureSource => _featureSource;
    }
}
