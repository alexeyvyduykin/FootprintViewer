using NetTopologySuite.Geometries;

namespace FootprintViewer.Data.Models;

public class FootprintGeometry
{
    public Point Center { get; set; } = null!;

    public LineString Border { get; set; } = null!;
}
