using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public abstract class InteractiveFeature : GeometryFeature
    {
        public InteractiveFeature(GeometryFeature feature) : base(feature) { }

        public abstract IList<MPoint> EditVertices();
    }
}
