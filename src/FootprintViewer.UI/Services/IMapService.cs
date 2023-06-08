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

    Navigator Navigator { get; }

    LayerStyleManager LayerStyle { get; }

    AreaOfInterest AOI { get; }

    T? GetProvider<T>() where T : IProvider;

    void AddLayerProvider(LayerType type, IProvider provider);

    void EnterFeature(ILayer? layer, IFeature? feature);

    void LeaveFeature(ILayer? layer);

    void SelectFeature(ILayer? layer, IFeature? feature);

    void UnselectFeature(ILayer? layer);
}
