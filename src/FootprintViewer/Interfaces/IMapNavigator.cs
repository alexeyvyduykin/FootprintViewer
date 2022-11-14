using Mapsui;
using NetTopologySuite.Geometries;

namespace FootprintViewer;

public interface IMapNavigator
{
    void FlyToFootprintPreview(Geometry? geometry);

    void FlyToFootprint(Coordinate center);

    void ZoomIn();

    void ZoomOut();

    INavigator? Navigator { get; set; }

    IReadOnlyViewport? Viewport { get; set; }
}
