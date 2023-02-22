using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

    [JsonProperty("PlannedSchedules")]
    public Dictionary<string, List<ITaskResult>> PlannedSchedules { get; set; } = new();
}
