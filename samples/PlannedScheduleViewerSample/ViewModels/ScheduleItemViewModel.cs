using FootprintViewer.Data.Models;
using System;

namespace PlannedScheduleViewerSample.ViewModels;

public class ScheduleItemViewModel : ViewModelBase
{
    public ScheduleItemViewModel(PlannedScheduleItem item)
    {
        TaskName = item.TaskName;
        Begin = item.Interval.Begin;
        Duration = item.Interval.Duration;
    }

    public string TaskName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }
}