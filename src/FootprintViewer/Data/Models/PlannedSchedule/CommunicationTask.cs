using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class CommunicationTask : ITask
{
    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("GroundStationName")]
    public string GroundStationName { get; set; } = string.Empty;
}
