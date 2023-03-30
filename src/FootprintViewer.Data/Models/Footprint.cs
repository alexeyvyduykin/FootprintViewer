using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class Footprint
{
    [JsonProperty("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("SatelliteName")]
    public string SatelliteName { get; set; } = null!;

    [JsonProperty("TargetName")]
    public string TargetName { get; set; } = null!;

    [JsonProperty("Center")]
    public Point Center { get; set; } = null!;

    [JsonProperty("Points")]
    public LineString Border { get; set; } = null!;

    [JsonProperty("Begin")]
    public DateTime Begin { get; set; }

    [JsonProperty("Duration")]
    public double Duration { get; set; }

    [JsonProperty("Node")]
    public int Node { get; set; }

    [JsonProperty("Direction")]
    public SwathDirection Direction { get; set; }
}