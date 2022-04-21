using Mapsui.Geometries;
using System;
using System.Collections.Generic;

namespace InteractiveGeometry
{
    public interface IInteractiveObject
    {
        event EventHandler? InvalidateLayer;

        IEnumerable<Point> GetActiveVertices();

        void Starting(Point worldPosition);

        void Moving(Point worldPosition);

        void Ending(Point worldPosition, Predicate<Point>? isEnd);

        void Hovering(Point worldPosition);
    }
}
