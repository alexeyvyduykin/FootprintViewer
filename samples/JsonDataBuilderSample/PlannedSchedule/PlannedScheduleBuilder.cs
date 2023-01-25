using FootprintViewer.Data;
using FootprintViewer.Data.Models;

namespace JsonDataBuilderSample;

public static class PlannedScheduleBuilder
{
    public static PlannedScheduleResult Build(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        var observationTasks = groundTargets.Select((s, index) => new ObservationTask()
        {
            Name = $"ObservationTask_{index + 1}",
            TargetName = s.Name ?? string.Empty
        }).OfType<ITask>().ToList();

        var communicationTasks = groundStations.Select((s, index) => new CommunicationTask()
        {
            Name = $"CommunicationTask_{index + 1}",
            GroundTargetName = s.Name ?? string.Empty
        }).OfType<ITask>().ToList();

        var tasks = observationTasks.Concat(communicationTasks).ToList();

        var observationTaskResults = BuildObservationTaskResults(satellites, footprints, tasks);
        var comminicationTaskResults = BuildCommunicationTaskResults(satellites, groundStations, footprints, tasks);

        var dict = new Dictionary<string, List<ITaskResult>>();

        foreach (var satName in observationTaskResults.Keys)
        {
            var list = observationTaskResults[satName].Concat(comminicationTaskResults[satName]).ToList();
            dict.Add(satName, list);
        }

        return new PlannedScheduleResult()
        {
            Tasks = tasks,
            PlannedSchedules = dict
        };
    }

    private static Dictionary<string, List<ITaskResult>> BuildObservationTaskResults(IList<Satellite> satellites, IList<Footprint> footprints, IList<ITask> tasks)
    {
        var taskResults = satellites.ToDictionary(s => s.Name ?? string.Empty, _ => new List<ITaskResult>());

        foreach (var item in footprints)
        {
            var satelliteName = item.SatelliteName ?? string.Empty;
            var targetName = item.TargetName;
            var interval = new Interval()
            {
                Begin = item.Begin,
                Duration = item.Duration
            };

            var task = tasks
                .Where(s => s is ObservationTask observationTask && Equals(observationTask.TargetName, targetName))
                .FirstOrDefault();

            var observationTaskResult = new ObservationTaskResult()
            {
                TaskName = task?.Name ?? string.Empty,
                Interval = interval,
                Footprint = new FootprintFrame()
                {
                    Center = item.Center ?? new(null),
                    Points = item.Points ?? new(null)
                }
            };

            taskResults[satelliteName].Add(observationTaskResult);
        }

        return taskResults;
    }

    private static Dictionary<string, List<ITaskResult>> BuildCommunicationTaskResults(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    {
        var minDate = footprints.Select(s => s.Begin).Min();
        var maxDate = footprints.Select(s => s.Begin.AddSeconds(s.Duration)).Max();
        var totalSeconds = (maxDate - minDate).TotalSeconds;
        var random = new Random();
        var dt1 = totalSeconds / 2.0;
        var dt2 = totalSeconds / 2.0;

        var taskResults = satellites.ToDictionary(s => s.Name ?? string.Empty, _ => new List<ITaskResult>());

        foreach (var sat in satellites)
        {
            var satelliteName = sat.Name ?? string.Empty;

            foreach (var gs in groundStations)
            {
                var task = tasks
                    .Where(s => s is CommunicationTask communicationTask && Equals(communicationTask.GroundTargetName, gs.Name))
                    .FirstOrDefault();

                var duration1 = (double)random.Next(10, 20);
                var begin1 = random.Next(0, (int)(dt1 - duration1));

                var duration2 = (double)random.Next(40, 60);
                var begin2 = random.Next((int)dt1, (int)(dt1 + dt2 - duration2));

                var interval1 = new Interval() { Begin = minDate.AddSeconds(begin1), Duration = duration1 };
                var interval2 = new Interval() { Begin = minDate.AddSeconds(begin2), Duration = duration2 };

                var communicationTaskResult1 = new CommunicationTaskResult()
                {
                    TaskName = task?.Name ?? string.Empty,
                    Interval = interval1,
                    Type = CommunicationType.Uplink,
                };

                var communicationTaskResult2 = new CommunicationTaskResult()
                {
                    TaskName = task?.Name ?? string.Empty,
                    Interval = interval2,
                    Type = CommunicationType.Downlink,
                };

                taskResults[satelliteName].Add(communicationTaskResult1);
                taskResults[satelliteName].Add(communicationTaskResult2);
            }
        }

        return taskResults;
    }

}
