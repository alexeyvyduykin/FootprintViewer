using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

public class TaskAvailability
{
    public string TaskName { get; set; } = string.Empty;

    public string SatelliteName { get; set; } = string.Empty;

    public List<Interval> Windows { get; set; } = new();
}