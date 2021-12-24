using Mapsui.Geometries;
using System;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity
{
    public abstract class BaseInteractiveObject : IInteractiveObject
    {
        public event EventHandler? InvalidateLayer;

        protected void Invalidate()
        {
            InvalidateLayer?.Invoke(this, EventArgs.Empty);
        }

        public abstract IEnumerable<Point> GetActiveVertices();

        protected IGeometry Copy(IGeometry geometry)
        {
            var g = geometry.Copy();
            var count = g.MainVertices().Count;
            g.MainVertices().RemoveAt(count - 1);
            return g;
        }

        public abstract void Starting(Point worldPosition);

        public abstract void Moving(Point worldPosition);

        public abstract void Ending(Point worldPosition, Predicate<Point>? isEnd);

        public abstract void Hovering(Point worldPosition);
    }
}
