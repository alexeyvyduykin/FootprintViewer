using FootprintViewer.Data.Models;
using System;

namespace PlannedScheduleViewerSample.ViewModels;

public class ObservationViewModel : ViewModelBase
{
    public ObservationViewModel(ObservationTaskResult result)
    {
        TaskName = result.TaskName;
        Begin = result.Interval.Begin;
        Duration = result.Interval.Duration;
        Center = $"Lon: {result.Footprint?.Center.X:F2}; Lat: {result.Footprint?.Center.Y:F2}";
    }

    public string TaskName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }

    public string Center { get; set; }
}