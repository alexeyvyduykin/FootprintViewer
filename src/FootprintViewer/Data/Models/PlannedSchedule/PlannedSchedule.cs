using System.Collections.Generic;

namespace FootprintViewer.Data.Models;

public class PlannedSchedule
{
    public List<PlannedScheduleItem> Items { get; set; } = new();

    public List<FootprintFrame>? Footprints { get; set; }
}


