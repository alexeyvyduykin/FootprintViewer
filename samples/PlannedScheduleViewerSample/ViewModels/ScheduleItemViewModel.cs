using FootprintViewer.Data.Models;
using System;

namespace PlannedScheduleViewerSample.ViewModels;

public class ScheduleItemViewModel : ViewModelBase
{
    public ScheduleItemViewModel(ObservationTaskResult result)
    {
        TaskName = result.TaskName;
        Begin = result.Interval.Begin;
        Duration = result.Interval.Duration;
    }

    public string TaskName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }
}