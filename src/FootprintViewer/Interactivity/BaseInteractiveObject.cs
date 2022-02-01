using Mapsui.Geometries;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Interactivity
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
            if (geometry is LineString)
            {
                return geometry.Copy();
            }
            else if (geometry is Polygon)
            {
                var g = geometry.Copy();
                var count = g.MainVertices().Count;
                g.MainVertices().RemoveAt(count - 1);
                return g;
            }
            else
            {
                throw new Exception();
            }
        }

        public abstract void Starting(Point worldPosition);

        public abstract void Moving(Point worldPosition);

        public abstract void Ending(Point worldPosition, Predicate<Point>? isEnd);

        public abstract void Hovering(Point worldPosition);
    }
}
