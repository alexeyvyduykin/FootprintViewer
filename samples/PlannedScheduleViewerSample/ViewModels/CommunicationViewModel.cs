using FootprintViewer.Data.Models;
using System;

namespace PlannedScheduleViewerSample.ViewModels;

public class CommunicationViewModel : ViewModelBase
{
    public CommunicationViewModel(CommunicationTaskResult result)
    {
        TaskName = result.TaskName;
        Begin = result.Interval.Begin;
        Duration = result.Interval.Duration;
        Type = result.Type.ToString();
    }

    public string TaskName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }

    public string Type { get; set; }
}
