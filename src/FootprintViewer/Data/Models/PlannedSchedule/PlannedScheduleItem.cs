using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

public class PlannedScheduleItem
{
    public string TaskName { get; set; } = string.Empty;

    public Interval Interval { get; set; } = new();

    public List<Interval>? Windows { get; set; }

    public Interval? Transition { get; set; }
}
