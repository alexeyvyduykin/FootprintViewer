using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public abstract class InteractiveFeature : Feature
    {
        public InteractiveFeature(IFeature feature) : base(feature) { }

        public abstract IList<Point> EditVertices();
    }
}
