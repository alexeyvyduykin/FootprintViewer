using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;

namespace FootprintViewer.Data;

[JsonObject]
public class GroundStation
{
    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Center")]
    public Point Center { get; set; } = new Point(0, 0);

    [JsonProperty("Angles")]
    public double[] Angles { get; set; } = Array.Empty<double>();
}
