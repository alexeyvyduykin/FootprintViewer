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

        var communicationTasks = groundStations.Select((s, index) => new ComunicationTask()
        {
            Name = $"CommunicationTask_{index + 1}",
            GroundTargetName = s.Name ?? string.Empty
        }).OfType<ITask>().ToList();

        var tasks = observationTasks.Concat(communicationTasks).ToList();

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

        return new PlannedScheduleResult()
        {
            Tasks = tasks,
            PlannedSchedules = taskResults
        };
    }
}
