using Mapsui.Geometries;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class InteractiveRoute : InteractiveFeature
    {
        public InteractiveRoute(IFeature feature) : base(feature) { }

        public override IList<Point> EditVertices()
        {
            if (Geometry != null)
            {
                return ((LineString)Geometry).Vertices;
            }

            return new List<Point>();
        }
    }
}
