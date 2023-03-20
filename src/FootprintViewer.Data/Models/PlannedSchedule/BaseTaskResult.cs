﻿using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public abstract class BaseTaskResult : ITaskResult
{
    [JsonProperty("TaskName")]
    public string TaskName { get; set; } = string.Empty;

    [JsonProperty("SatelliteName")]
    public string SatelliteName { get; set; } = string.Empty;

    [JsonProperty("Interval")]
    public Interval Interval { get; set; } = new();

    [JsonProperty("Transition")]
    public Interval? Transition { get; set; }
}