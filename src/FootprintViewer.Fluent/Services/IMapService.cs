using FootprintViewer.Factories;
using FootprintViewer.Models;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Providers;

namespace FootprintViewer.Fluent.Services2;

public interface IMapService
{
    Map Map { get; }

    MapState State { get; }

    INavigator? Navigator { get; }

    IReadOnlyViewport? Viewport { get; }

    LayerStyleManager LayerStyle { get; }

    AreaOfInterest AOI { get; }

    T? GetProvider<T>() where T : IProvider;

    void SetNavigator(INavigator navigator);

    void SetViewport(IReadOnlyViewport viewport);

    void AddLayerProvider(LayerType type, IProvider provider);
}
