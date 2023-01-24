using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

public enum CommunicationType
{
    Uplink,
    Downlink
}

[JsonObject]
public class CommunicationTask : ITask
{
    [JsonProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("GroundTargetName")]
    public string GroundTargetName { get; set; } = string.Empty;

    //public CommunicationType CommunicationType { get; set; }
}
