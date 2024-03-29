﻿using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class GroundStation
{
    [JsonProperty("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("Center")]
    public Point Center { get; set; } = new Point(0, 0);

    [JsonProperty("Angles")]
    public double[] Angles { get; set; } = Array.Empty<double>();
}
