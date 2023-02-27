using FootprintViewer.Data.Models;
using System;

namespace FootprintViewer.ViewModels.SidePanel.Items;

public sealed class TaskResultViewModel : ViewModelBase
{
    public TaskResultViewModel(string satelliteName, ITaskResult taskResult)
    {
        Model = taskResult;

        SatelliteName = satelliteName;
        TaskName = taskResult.TaskName;
        Begin = taskResult.Interval.Begin;
        Duration = taskResult.Interval.Duration;
    }

    public string TaskName { get; set; }

    public string SatelliteName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }

    public ITaskResult Model { get; set; }

    //public List<Interval>? Windows { get; set; }

    //public Interval? Transition { get; set; }
}
