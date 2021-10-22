using Mapsui;
using System;
using System.Collections.Generic;
using System.Text;
using Mapsui.Geometries;
namespace FootprintViewer.Graphics
{
    public class ConcreteCircle : Concrete
    {
        private InteractiveCircle _circle;
        public override IInteractiveFeature CreateConcrete()
        {
            _circle = new InteractiveCircle(null);
            return _circle;
        }
        public override bool IsEndDrawing(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if(_circle != null)
            {
                return true;
            }

            throw new Exception();
        }
    }
}
