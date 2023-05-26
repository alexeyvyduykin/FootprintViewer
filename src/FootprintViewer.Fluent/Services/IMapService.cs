using FootprintViewer.Factories;
using FootprintViewer.Models;
using FootprintViewer.StateMachines;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;

namespace FootprintViewer.UI.Services2;

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

    void EnterFeature(ILayer? layer, IFeature? feature);

    void LeaveFeature(ILayer? layer);

    void SelectFeature(ILayer? layer, IFeature? feature);

    void UnselectFeature(ILayer? layer);
}
