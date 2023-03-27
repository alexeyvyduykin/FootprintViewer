using Mapsui;
using System;

namespace SpaceScienceSample.Models;

public interface IMapNavigator
{
    void ZoomIn();

    void ZoomOut();

    void Click(MPoint worldPosition);

    IObservable<(double lonDeg, double latDeg)> ClickObservable { get; }

    INavigator? Navigator { get; set; }

    IReadOnlyViewport? Viewport { get; set; }
}
