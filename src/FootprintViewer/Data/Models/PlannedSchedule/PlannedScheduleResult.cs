using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

public class PlannedScheduleResult
{
    public List<ITask> Tasks { get; set; } = new();

    public Dictionary<string, PlannedSchedule> PlannedSchedules { get; set; } = new();
}
