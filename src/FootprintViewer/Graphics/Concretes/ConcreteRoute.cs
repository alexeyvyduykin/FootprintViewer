using Mapsui;
using System;
using System.Collections.Generic;
using System.Text;
using Mapsui.Geometries;
namespace FootprintViewer.Graphics
{
    public class ConcreteRoute : Concrete
    {
        private InteractiveRoute _route;
        private readonly int _minPixelsMovedForDrag = 4;

        public override IInteractiveFeature CreateConcrete()
        {
            _route = new InteractiveRoute(null);

            return _route;
        }

        public override bool IsEndDrawing(Point worldPosition, Point screenPosition, IReadOnlyViewport viewport)
        {
            if (_route != null)
            {
                var routeGeometry = (LineString)_route.Geometry;

                if (routeGeometry.Vertices.Count > 1)
                {
                    // is end?
                    foreach (var item in routeGeometry.Vertices)
                    {
                        var p = viewport.WorldToScreen(item);

                        if (IsClick(p, screenPosition) == true)
                        {                         
                            return true;
                        }
                    }
                }

                return false;
            }

            throw new Exception();
        }
        private bool IsClick(Point screenPosition, Point mouseDownScreenPosition)
        {
            if (mouseDownScreenPosition == null || screenPosition == null)
            {
                return false;
            }

            return mouseDownScreenPosition.Distance(screenPosition) < _minPixelsMovedForDrag;
        }
    }
}
