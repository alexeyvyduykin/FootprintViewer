using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Providers;

namespace FootprintViewer.Fluent.Services2;

public interface IMapService
{
    IMap Map { get; }

    void SetNavigator(INavigator navigator);

    void SetViewport(IReadOnlyViewport viewport);

    void AddLayerProvider(LayerType type, IProvider provider);
}
