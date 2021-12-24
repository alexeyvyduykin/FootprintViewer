using Mapsui.Geometries;
using System;

namespace InteractivitySample.Interactivity
{
    public interface IInteractiveObject
    {
        event EventHandler? InvalidateLayer;

        void Starting(Point worldPosition);

        void Moving(Point worldPosition);

        void Ending(Point worldPosition, Predicate<Point>? isEnd);

        void Hovering(Point worldPosition);
    }
}
