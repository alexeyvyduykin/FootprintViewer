using FootprintViewer.Data.Models;
using Mapsui;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Geometries;

public static partial class FeatureBuilder
{
    public static List<IFeature> CreateFootprints(IEnumerable<Footprint> footprints)
    {
        return footprints.Select(s => CreateFootprint(s)).ToList();
    }

    public static IFeature CreateFootprint(Footprint footprint)
    {
        var poly = AreaCutting(footprint.Border.Coordinates);

        var feature = poly.ToFeatureEx(footprint.Name);

        return feature;
    }
}
