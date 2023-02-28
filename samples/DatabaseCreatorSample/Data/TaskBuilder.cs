using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data;

public static class TaskBuilder
{
    public static List<ITask> CreateTasks(IList<GroundTarget>? groundTargets)
    {
        return groundTargets?
            .Select((s, i) => (ITask)new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = s.Name!
            }).ToList() ?? new();
    }
}
