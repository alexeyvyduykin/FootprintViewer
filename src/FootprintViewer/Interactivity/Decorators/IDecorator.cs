using Mapsui.Providers;

namespace FootprintViewer.Interactivity.Decorators
{
    public interface IDecorator : IInteractiveObject
    {
        IFeature FeatureSource { get; }
    }
}
