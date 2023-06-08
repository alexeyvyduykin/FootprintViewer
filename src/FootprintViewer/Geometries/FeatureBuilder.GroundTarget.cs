using FootprintViewer.Data.Models;
using Mapsui;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Geometries;

public static partial class FeatureBuilder
{
    public static List<IFeature> CreateGroundTargets(IEnumerable<GroundTarget> groundTargets)
    {
        return groundTargets.Select(s => CreateGroundTarget(s)).ToList();
    }

    public static IFeature CreateGroundTarget(GroundTarget groundTarget)
    {
        var geometry = groundTarget.Type switch
        {
            GroundTargetType.Point => new Point(SphericalMercator.FromLonLat(((Point)groundTarget.Points!).X, ((Point)groundTarget.Points!).Y).ToCoordinate()),
            GroundTargetType.Route => RouteCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
            GroundTargetType.Area => AreaCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList()),
            _ => throw new Exception()
        };

        var feature = geometry.ToFeature();

        feature["Name"] = groundTarget.Name;
        feature["State"] = "Unselected";
        feature["Highlight"] = false;
        feature["Type"] = groundTarget.Type.ToString();

        return feature;
    }
}
