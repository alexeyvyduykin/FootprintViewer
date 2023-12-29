using FootprintViewer.Data.Models;
using Newtonsoft.Json;

namespace JsonDataBuilderSample.Models;

[JsonObject]
public class PlannedScheduleObject
{
    [JsonIgnore]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime DateTime { get; set; }

    [JsonProperty("Satellites")]
    public List<Satellite> Satellites { get; set; } = new();

    [JsonProperty("GroundTargets")]
    public List<GroundTarget> GroundTargets { get; set; } = new();

    [JsonProperty("GroundStations")]
    public List<GroundStation> GroundStations { get; set; } = new();

    [JsonProperty("ObservationTasks")]
    public List<ObservationTask> ObservationTasks { get; set; } = new();

    [JsonProperty("CommunicationTasks")]
    public List<CommunicationTask> CommunicationTasks { get; set; } = new();

    [JsonProperty("ObservationWindows")]
    public List<TaskAvailability> ObservationWindows { get; set; } = new();

    [JsonProperty("CommunicationWindows")]
    public List<TaskAvailability> CommunicationWindows { get; set; } = new();

    [JsonProperty("ObservationTaskResults")]
    public List<ObservationTaskResult> ObservationTaskResults { get; set; } = new();

    [JsonProperty("CommunicationTaskResults")]
    public List<CommunicationTaskResult> CommunicationTaskResults { get; set; } = new();
}
