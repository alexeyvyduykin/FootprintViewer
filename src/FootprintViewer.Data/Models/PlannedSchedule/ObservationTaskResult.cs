using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class ObservationTaskResult : BaseTaskResult
{
    [JsonProperty("Geometry")]
    public FootprintGeometry Geometry { get; set; } = null!;

    [JsonProperty("Direction")]
    public SwathDirection Direction { get; set; }
}
