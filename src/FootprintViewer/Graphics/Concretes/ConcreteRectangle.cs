using Mapsui;
using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Graphics
{
    public class ConcreteRectangle : Concrete
    {
        private InteractiveRectangle _rectangle;
        public override IInteractiveFeature CreateConcrete()
        {
            _rectangle = new InteractiveRectangle(null);
            return _rectangle;
        }

        public override bool IsEndDrawing(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_rectangle != null)
            {
                return true;
            }

            throw new Exception();
        }
    }
}
