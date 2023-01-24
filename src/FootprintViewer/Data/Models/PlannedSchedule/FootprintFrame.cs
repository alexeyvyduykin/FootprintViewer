using NetTopologySuite.Geometries;

namespace FootprintViewer.Data.Models;

public class FootprintFrame
{
    public string TaskName { get; set; } = string.Empty;

    public Point Center { get; set; } = Point.Empty;

    public LineString Points { get; set; } = LineString.Empty;
}
