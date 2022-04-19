using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class InteractiveCircle : InteractiveFeature
    {
        private readonly MPoint _center;

        public InteractiveCircle(GeometryFeature feature) : base(feature)
        {
            _center = feature.Geometry.Centroid.ToMPoint();// BoundingBox.Centroid;
        }

        public override IList<MPoint> EditVertices() => new List<MPoint>() { _center };
    }
}
