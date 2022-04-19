using Mapsui;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity
{
    public interface IInteractiveObject
    {
        event EventHandler? InvalidateLayer;

        IEnumerable<MPoint> GetActiveVertices();

        void Starting(MPoint worldPosition);

        void Moving(MPoint worldPosition);

        void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd);

        void Hovering(MPoint worldPosition);
    }
}
