using Mapsui.Providers;

namespace InteractiveGeometry
{
    public interface IDecorator : IInteractiveObject
    {
        IFeature FeatureSource { get; }
    }
}
