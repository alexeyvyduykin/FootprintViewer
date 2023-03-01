using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

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
