using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveGeometry.UI.Input
{
    public class DefaultManipulator : MouseManipulator
    {
        public DefaultManipulator(IMapView mapView) : base(mapView)
        {
            MapView.SetCursor(CursorType.Default);
        }
    }
}
