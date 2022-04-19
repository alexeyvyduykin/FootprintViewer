using Mapsui;
using NetTopologySuite.Geometries;
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

        public abstract IEnumerable<MPoint> GetActiveVertices();

        protected Geometry Copy(Geometry geometry)
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
            else if (geometry is Point)
            {
                return geometry.Copy();
            }
            else
            {
                throw new Exception();            
            }
        }

        public abstract void Starting(MPoint worldPosition);

        public abstract void Moving(MPoint worldPosition);

        public abstract void Ending(MPoint worldPosition, Predicate<MPoint>? isEnd);

        public abstract void Hovering(MPoint worldPosition);
    }
}
