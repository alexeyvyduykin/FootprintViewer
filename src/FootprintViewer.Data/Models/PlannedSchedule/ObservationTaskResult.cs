using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class ObservationTaskResult : BaseTaskResult
{
    [JsonProperty("Footprint")]
    public FootprintFrame? Footprint { get; set; }
}
