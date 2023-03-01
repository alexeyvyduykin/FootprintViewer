using FootprintViewer.Data.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data;

public static class ModelBuilder
{
    public static async Task<List<ITaskResult>> CreateObservationTasksAsync(IList<ITask> tasks, IList<Footprint> footprints)
    {
        return await Observable.Start(() => RandomModelBuilder.BuildObservationTaskResults(tasks, footprints), RxApp.TaskpoolScheduler);
    }
}
