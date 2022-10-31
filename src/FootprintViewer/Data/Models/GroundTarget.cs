using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data;

public enum GroundTargetType
{
    Point,
    Route,
    Area
}

[JsonObject]
public class GroundTarget
{
    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Type")]
    public GroundTargetType Type { get; set; }

    [JsonProperty("Points")]
    public Geometry? Points { get; set; }
}
