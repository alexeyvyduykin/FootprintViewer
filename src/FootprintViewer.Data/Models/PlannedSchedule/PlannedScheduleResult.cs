using Newtonsoft.Json;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class PlannedScheduleResult
{
    [JsonIgnore]
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime DateTime { get; set; }

    [JsonProperty("Tasks")]
    public List<ITask> Tasks { get; set; } = new();

    [JsonProperty("TaskAvailabilities")]
    public List<TaskAvailability> TaskAvailabilities { get; set; } = new();

    [JsonProperty("PlannedSchedules")]
    public List<ITaskResult> PlannedSchedules { get; set; } = new();
}
