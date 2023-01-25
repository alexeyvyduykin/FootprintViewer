using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

public class FootprintFrame
{
    public Point Center { get; set; } = Point.Empty;

    public LineString Points { get; set; } = LineString.Empty;
}

[JsonObject]
public class ObservationTaskResult : BaseTaskResult
{
    [JsonProperty("Footprint")]
    public FootprintFrame? Footprint { get; set; }
}
