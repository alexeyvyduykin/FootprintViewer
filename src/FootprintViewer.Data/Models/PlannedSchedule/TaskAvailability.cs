using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class TaskAvailability
{
    [JsonProperty("TaskName")]
    public string TaskName { get; set; } = string.Empty;

    [JsonProperty("SatelliteName")]
    public string SatelliteName { get; set; } = string.Empty;

    [JsonProperty("Windows")]
    public List<Interval> Windows { get; set; } = new();
}