using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using ReactiveUI.Fody.Helpers;
using System.Linq;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Items;

public sealed class FootprintViewModel : ViewModelBase, IViewerItem
{
    private readonly Footprint? _footprint;
    private readonly string _name;
    private readonly string _satelliteName;
    private readonly Coordinate _center;
    private readonly Polygon? _polygon;
    private readonly DateTime _begin;
    private readonly double _duration;
    private readonly int _node;
    private readonly SwathDirection _direction;

    public FootprintViewModel() : this(FootprintBuilder.CreateRandom()) { }

    public FootprintViewModel(Footprint footprint)
    {
        _footprint = footprint;
        _name = footprint.Name!;
        _satelliteName = footprint.SatelliteName!;
        _center = footprint.Center!.Coordinate.Copy();
        _begin = footprint.Begin;
        _duration = footprint.Duration;
        _node = footprint.Node;
        _direction = footprint.Direction;
        _polygon = CreatePolygon(footprint.Border);
    }

    private static Polygon? CreatePolygon(LineString? lineString)
    {
        if (lineString != null)
        {
            var list = lineString.Coordinates.ToList();
            var first = lineString.Coordinates.First();
            list.Add(first);

            var points = list
                .Select(s => SphericalMercator.FromLonLat(s.X, s.Y))
                .Select(s => new Coordinate(s.x, s.y))
                .ToArray();

            var linearRing = new LinearRing(points);

            return new Polygon(linearRing);
        }

        return null;
    }

    // TODO: make not nullable
    public Footprint? Footprint => _footprint;

    public string Name => _name;

    public string SatelliteName => _satelliteName;

    public Coordinate Center => _center;

    public Polygon? Polygon => _polygon;

    public DateTime Begin => _begin;

    public double Duration => _duration;

    public int Node => _node;

    public SwathDirection Direction => _direction;

    [Reactive]
    public bool IsShowInfo { get; set; } = false;
}

