using Newtonsoft.Json;
using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

[JsonObject]
public abstract class BaseTaskResult : ITaskResult
{
    [JsonProperty("TaskName")]
    public string TaskName { get; set; } = string.Empty;

    [JsonProperty("Interval")]
    public Interval Interval { get; set; } = new();

    [JsonProperty("Windows")]
    public List<Interval>? Windows { get; set; }

    [JsonProperty("Transition")]
    public Interval? Transition { get; set; }
}
