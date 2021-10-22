using Mapsui;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Graphics
{
    public abstract class Concrete
    {         
        public abstract IInteractiveFeature CreateConcrete();

        public abstract bool IsEndDrawing(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport);
    }
}
