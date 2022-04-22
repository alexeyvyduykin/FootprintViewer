using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using System.Linq;

namespace InteractiveGeometry
{
    public class InteractiveFactory
    {
        public IDesigner CreatePolygonDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new PolygonDesigner());
        }

        public IDesigner CreateRouteDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new RouteDesigner());
        }

        public IDesigner CreateCircleDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new CircleDesigner());
        }

        public IDesigner CreateRectangleDesigner(IMap map, WritableLayer source)
        {
            return CreateDesigner(map, source, new RectangleDesigner());
        }

        private IDesigner CreateDesigner(IMap map, WritableLayer source, IDesigner designer)
        {
            var interactiveLayer = map.Layers.FindLayer(nameof(InteractiveLayer)).FirstOrDefault();

            if (interactiveLayer != null)
            {
                map.Layers.Remove(interactiveLayer);
            }

            interactiveLayer = new InteractiveLayer(designer) { Name = nameof(InteractiveLayer) };

            designer.BeginCreating += (s, e) =>
            {
                map.Layers.Add(interactiveLayer);
            };

            designer.EndCreating += (s, e) =>
            {
                source.Add(designer.Feature.Copy());

                map.Layers.Remove(interactiveLayer);
            };

            return designer;
        }
    }
}
