using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class InteractiveRoute : InteractiveFeature
    {
        public InteractiveRoute(GeometryFeature feature) : base(feature) { }

        public override IList<MPoint> EditVertices()
        {
            if (Geometry != null)
            {
                return ((LineString)Geometry).Coordinates.Select(s => s.ToMPoint()).ToList();// Vertices;
            }

            return new List<MPoint>();
        }
    }
}
