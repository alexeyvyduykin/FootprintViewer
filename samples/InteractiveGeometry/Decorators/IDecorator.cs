using Mapsui.Nts;

namespace InteractiveGeometry
{
    public interface IDecorator : IInteractiveObject
    {
        GeometryFeature FeatureSource { get; }
    }
}
