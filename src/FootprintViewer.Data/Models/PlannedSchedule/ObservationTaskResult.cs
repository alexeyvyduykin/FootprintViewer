using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class ObservationTaskResult : BaseTaskResult
{
    [JsonProperty("TargetName")]
    public string TargetName { get; set; } = null!;

    [JsonProperty("Geometry")]
    public FootprintGeometry Geometry { get; set; } = null!;

    [JsonProperty("Direction")]
    public SwathDirection Direction { get; set; }
}
