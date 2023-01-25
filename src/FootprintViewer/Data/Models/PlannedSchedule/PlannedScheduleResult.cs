using Newtonsoft.Json;
using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

[JsonObject]
public class PlannedScheduleResult
{
    [JsonProperty("Tasks")]
    public List<ITask> Tasks { get; set; } = new();

    [JsonProperty("PlannedSchedules")]
    public Dictionary<string, List<ITaskResult>> PlannedSchedules { get; set; } = new();
}
