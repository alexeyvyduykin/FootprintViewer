using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using FootprintViewer.Helpers;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Models;

public class CustomSource : ISource
{
    public async Task<IList<object>> GetValuesAsync()
    {
        return await Observable.Start(() =>
        {
            string path = Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "PlannedSchedule.json");

            return new List<object>() { (PlannedScheduleResult)JsonHelpers.DeserializeFromFile<PlannedScheduleResult>(path)! };
        }, RxApp.TaskpoolScheduler);
    }
}
