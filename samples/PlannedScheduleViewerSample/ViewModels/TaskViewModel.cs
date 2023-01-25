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
            Description = observationTask.TargetName;
        }
        else if (task is ComunicationTask communicationTask)
        {
            Description = communicationTask.GroundTargetName;
        }
    }

    public string Name { get; set; }

    public string Description { get; set; }
}
