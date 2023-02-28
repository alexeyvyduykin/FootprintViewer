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

        var observationTaskResults = BuildObservationTaskResults(footprints, tasks);
        var comminicationTaskResults = BuildCommunicationTaskResults(satellites, groundStations, footprints, tasks);

        var list = observationTaskResults.Concat(comminicationTaskResults).ToList();

        return new PlannedScheduleResult()
        {
            Tasks = tasks,
            PlannedSchedules = list
        };
    }

    private static List<ITaskResult> BuildObservationTaskResults(IList<Footprint> footprints, IList<ITask> tasks)
    {
        var taskResults = new List<ITaskResult>();

        foreach (var item in footprints)
        {
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
                SatelliteName = item.SatelliteName ?? "SatelliteDefault",
                Interval = interval,
                Footprint = new FootprintFrame()
                {
                    Center = item.Center ?? new(null),
                    Points = item.Points ?? new(null)
                }
            };

            taskResults.Add(observationTaskResult);
        }

        return taskResults;
    }

    private static List<ITaskResult> BuildCommunicationTaskResults(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    {
        var minDate = footprints.Select(s => s.Begin).Min();
        var maxDate = footprints.Select(s => s.Begin.AddSeconds(s.Duration)).Max();
        var totalSeconds = (maxDate - minDate).TotalSeconds;
        var random = new Random();
        var dt1 = totalSeconds / 2.0;
        var dt2 = totalSeconds / 2.0;

        var taskResults = new List<ITaskResult>();

        foreach (var satName in satellites.Select(s => s.Name ?? "SatelliteDefault"))
        {
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
                    SatelliteName = satName,
                    Interval = interval1,
                    Type = CommunicationType.Uplink,
                };

                var communicationTaskResult2 = new CommunicationTaskResult()
                {
                    TaskName = task?.Name ?? string.Empty,
                    SatelliteName = satName,
                    Interval = interval2,
                    Type = CommunicationType.Downlink,
                };

                taskResults.Add(communicationTaskResult1);
                taskResults.Add(communicationTaskResult2);
            }
        }

        return taskResults;
    }
}
