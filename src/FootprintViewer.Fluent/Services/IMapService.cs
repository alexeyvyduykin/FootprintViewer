using FootprintViewer.Factories;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;

namespace FootprintViewer.Fluent.Services2;

public interface IMapService
{
    Map Map { get; }

    INavigator? Navigator { get; }

    IReadOnlyViewport? Viewport { get; }

    T? GetProvider<T>() where T : IProvider;

    LayerStyleManager LayerStyle { get; }

    void SetNavigator(INavigator navigator);

    void SetViewport(IReadOnlyViewport viewport);

    void AddLayerProvider(LayerType type, IProvider provider);
}
