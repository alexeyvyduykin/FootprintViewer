using Mapsui.Nts;

namespace InteractiveGeometry
{
    public interface IDecorator : IInteractive
    {
        GeometryFeature FeatureSource { get; }
    }
}
