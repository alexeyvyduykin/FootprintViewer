using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class ObservationTask : ITask
{
    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("TargetName")]
    public string TargetName { get; set; } = string.Empty;
}
