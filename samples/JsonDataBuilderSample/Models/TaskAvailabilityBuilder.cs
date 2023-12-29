using FootprintViewer.Data.Models;
using SpaceScience;
using System.Reactive.Linq;

namespace JsonDataBuilderSample.Models;

public static class TaskAvailabilityBuilder
{
    public static List<TaskAvailability> CreateObservations(IList<ObservationTask> tasks, IList<Satellite> satellites, IList<(string satName, IList<TimeWindowResult> windows)> windows)
    {
        var list = new List<TaskAvailability>();

        foreach (var task in tasks)
        {
            var taskName = task.Name;
            var gtName = task.GroundTargetName;

            var res = windows.Select(s => (s.satName, windows: s.windows.Where(t => Equals(t.Name, gtName)).ToList())).ToList();

            foreach (var (satName, windowResults) in res)
            {
                var sat = satellites.Where(s => Equals(s.Name, satName)).Single();

                var epoch = sat.Epoch;
                var period = sat.Period;

                var taskAvailability = new TaskAvailability()
                {
                    TaskName = taskName,
                    SatelliteName = satName,
                    Windows = windowResults.Select(s => CreateInterval(s, epoch, period)).ToList()
                };

                list.Add(taskAvailability);
            }
        }

        return list;

        static Interval CreateInterval(TimeWindowResult window, DateTime epoch, double period)
        {
            var duration = (window.BeginNode == window.EndNode)
                ? window.EndTime - window.BeginTime
                : period - window.BeginTime + window.EndTime;

            return new Interval()
            {
                Begin = epoch.AddSeconds(window.BeginTime + window.BeginNode * period),
                Duration = duration
            };
        }
    }
}
