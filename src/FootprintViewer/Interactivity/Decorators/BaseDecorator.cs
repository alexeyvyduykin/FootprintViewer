using Mapsui.Nts;

namespace FootprintViewer.Interactivity.Decorators
{
    public abstract class BaseDecorator : BaseInteractiveObject, IDecorator
    {
        private readonly GeometryFeature _featureSource;

        public BaseDecorator(GeometryFeature featureSource)
        {
            _featureSource = featureSource;
        }

        public GeometryFeature FeatureSource => _featureSource;
    }
}
