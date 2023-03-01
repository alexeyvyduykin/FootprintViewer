using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class CommunicationTaskResult : BaseTaskResult
{
    [JsonProperty("Type")]
    public CommunicationType Type { get; set; }
}
