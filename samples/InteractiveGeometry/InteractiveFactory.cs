using Mapsui;
using Mapsui.Layers;
using System.Linq;

namespace InteractiveGeometry
{
    public class InteractiveFactory
    {
        public IDesigner CreatePolygonDesigner(IMap map, ILayer layer)
        {
            return CreateDesigner(map, layer, new PolygonDesigner());
        }

        public IDesigner CreateRouteDesigner(IMap map, ILayer layer)
        {
            return CreateDesigner(map, layer, new RouteDesigner());
        }

        public IDesigner CreateCircleDesigner(IMap map, ILayer layer)
        {
            return CreateDesigner(map, layer, new CircleDesigner());
        }

        public IDesigner CreateRectangleDesigner(IMap map, ILayer layer)
        {
            return CreateDesigner(map, layer, new RectangleDesigner());
        }

        private IDesigner CreateDesigner(IMap map, ILayer layer, IDesigner designer)
        {
            var interactiveLayer = map.Layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                map.Layers.Remove(interactiveLayer);
            }

            interactiveLayer = new InteractiveLayer(layer, designer) { Name = nameof(InteractiveLayer) };

            designer.BeginCreating += (s, e) => 
            {
                map.Layers.Add(interactiveLayer);
            };
          
            designer.EndCreating += (s, e) => 
            {
                map.Layers.Remove(interactiveLayer);
            };

            return designer;
        }
    }
}
