using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using Mapsui.Interactivity;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Linq;

namespace FootprintViewer.UI.ViewModels.SidePanel.Items;

public sealed class GroundTargetViewModel : ViewModelBase, ISelectorItem, IViewerItem
{
    private readonly GroundTarget _groundTarget;
    private readonly GroundTargetType _type;
    private readonly string _name;
    private readonly Geometry? _geometry;

    public GroundTargetViewModel() : this(GroundTargetBuilder.CreateRandom()) { }

    public GroundTargetViewModel(GroundTarget groundTarget)
    {
        _groundTarget = groundTarget;
        _type = groundTarget.Type;
        _name = groundTarget.Name!;
        _geometry = CreateGeometry(groundTarget.Type, groundTarget.Points);
        Key = _type.ToString();
    }

    private static Geometry? CreateGeometry(GroundTargetType type, Geometry? geometry)
    {
        var cordinates = geometry?
            .MainVertices()
            .Select(s => SphericalMercator.FromLonLat(s.X, s.Y))
            .Select(s => new Coordinate(s.x, s.y))
            .ToArray();

        return type switch
        {
            GroundTargetType.Point => new Point(cordinates?.First()),
            GroundTargetType.Route => new LineString(cordinates),
            GroundTargetType.Area => new Polygon(new LinearRing(cordinates?.ToClosedCoordinates())),
            _ => null,
        };
    }

    public string? Key { get; set; }// GetKey() => _type.ToString();

    public GroundTarget GroundTarget => _groundTarget;

    public Geometry? Geometry => _geometry;

    public string Name => _name;

    public GroundTargetType Type => _type;

    public bool IsShowInfo { get; set; }
}
