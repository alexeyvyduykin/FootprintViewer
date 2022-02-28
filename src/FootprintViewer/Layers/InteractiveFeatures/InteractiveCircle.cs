using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class InteractiveCircle : InteractiveFeature
    {
        private readonly Point _center;

        public InteractiveCircle(IFeature feature) : base(feature)
        {
            _center = feature.Geometry.BoundingBox.Centroid;
        }

        public override IList<Point> EditVertices() => new List<Point>() { _center };
    }
}
