using Mapsui;

namespace SpaceScienceSample.Models;

public interface IMapNavigator
{
    void ZoomIn();

    void ZoomOut();

    INavigator? Navigator { get; set; }

    IReadOnlyViewport? Viewport { get; set; }
}
