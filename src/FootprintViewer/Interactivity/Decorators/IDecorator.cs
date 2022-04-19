using Mapsui.Nts;

namespace FootprintViewer.Interactivity.Decorators
{
    public interface IDecorator : IInteractiveObject
    {
        GeometryFeature FeatureSource { get; }
    }
}
