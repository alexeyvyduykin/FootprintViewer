using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class InteractiveCircle : InteractiveFeature
    {
        private readonly MPoint _center = new();

        public InteractiveCircle(GeometryFeature feature) : base(feature)
        {
            if (Geometry != null)
            {
                _center = Geometry.Centroid.ToMPoint();
            }
        }

        public override IList<MPoint> EditVertices() => new List<MPoint>() { _center };
    }
}
