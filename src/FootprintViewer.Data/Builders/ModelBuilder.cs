using FootprintViewer.Data.Models;
using ReactiveUI;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class ModelBuilder
{
    public static async Task<List<ITaskResult>> CreateObservationTasksAsync(IList<ITask> tasks, IList<Footprint> footprints)
    {
        return await Observable.Start(() => RandomModelBuilder.BuildObservationTaskResults(tasks, footprints), RxApp.TaskpoolScheduler);
    }
}
