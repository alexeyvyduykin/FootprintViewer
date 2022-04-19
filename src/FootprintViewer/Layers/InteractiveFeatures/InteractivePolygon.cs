using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class InteractivePolygon : InteractiveFeature
    {
        public InteractivePolygon(GeometryFeature feature) : base(feature) { }

        public override IList<MPoint> EditVertices()
        {
            if (Geometry != null)
            {
                return ((Polygon)Geometry).ExteriorRing.Coordinates.Select(s => s.ToMPoint()).ToList();// Vertices;
            }

            return new List<MPoint>();
        }
    }
}
