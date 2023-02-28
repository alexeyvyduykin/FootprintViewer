using FootprintViewer.Data.Models;

namespace PlannedScheduleViewerSample.ViewModels;

public class TaskViewModel : ViewModelBase
{
    public TaskViewModel(ITask task)
    {
        Name = task.Name;

        Description = string.Empty;

        if (task is ObservationTask observationTask)
        {
            Description = observationTask.GroundTargetName;
        }
        else if (task is CommunicationTask communicationTask)
        {
            Description = communicationTask.GroundStationName;
        }
    }

    public string Name { get; set; }

    public string Description { get; set; }
}
