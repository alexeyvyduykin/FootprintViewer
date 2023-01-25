using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

public enum CommunicationType
{
    Uplink,
    Downlink
}

[JsonObject]
public class ComunicationTaskResult : BaseTaskResult
{
    [JsonProperty("Type")]
    public CommunicationType Type { get; set; }
}
