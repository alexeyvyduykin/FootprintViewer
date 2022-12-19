﻿using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;

namespace FootprintViewer.Data;

public enum SwathDirection
{
    Left,
    Right
}

[JsonObject]
public class Footprint
{
    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("SatelliteName")]
    public string? SatelliteName { get; set; }

    [JsonProperty("TargetName")]
    public string? TargetName { get; set; }

    [JsonProperty("Center")]
    public Point? Center { get; set; }

    [JsonProperty("Points")]
    public LineString? Points { get; set; }

    [JsonProperty("Begin")]
    public DateTime Begin { get; set; }

    [JsonProperty("Duration")]
    public double Duration { get; set; }

    [JsonProperty("Node")]
    public int Node { get; set; }

    [JsonProperty("Direction")]
    public SwathDirection Direction { get; set; }
}