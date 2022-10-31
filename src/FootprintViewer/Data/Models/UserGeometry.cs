using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data;

public enum UserGeometryType { Point, Rectangle, Polygon, Circle }

[JsonObject]
public class UserGeometry
{
    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Type")]
    public UserGeometryType Type { get; set; }

    [JsonProperty("Geometry")]
    public Geometry? Geometry { get; set; }
}
