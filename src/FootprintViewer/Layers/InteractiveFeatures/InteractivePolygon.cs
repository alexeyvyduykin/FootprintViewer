using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class InteractivePolygon : InteractiveFeature
    {
        public InteractivePolygon(IFeature feature) : base(feature) { }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                return ((Polygon)Geometry).ExteriorRing.Vertices;
            }

            return new List<Point>();
        }
    }
}
