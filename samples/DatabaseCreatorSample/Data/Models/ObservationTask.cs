using FootprintViewer.Data;
using NetTopologySuite.Geometries;

namespace DatabaseCreatorSample.Data.Models;

public class ObservationTask : BaseTask
{
    public string? FootprintName { get; set; }

    public string? GroundTargetName { get; set; }

    public Point? Center { get; set; }

    public LineString? Points { get; set; }

    public SwathDirection Direction { get; set; }
}
