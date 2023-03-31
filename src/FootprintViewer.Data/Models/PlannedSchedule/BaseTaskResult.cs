using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public abstract class BaseTaskResult : ITaskResult
{
    [JsonProperty("Name")]
    public string Name { get; set; } = null!;

    [JsonProperty("TaskName")]
    public string TaskName { get; set; } = null!;

    [JsonProperty("SatelliteName")]
    public string SatelliteName { get; set; } = null!;

    [JsonProperty("Interval")]
    public Interval Interval { get; set; } = null!;

    [JsonProperty("Node")]
    public int Node { get; set; }

    [JsonProperty("Transition")]
    public Interval? Transition { get; set; }
}