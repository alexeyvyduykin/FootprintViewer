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

        var plannedScheduleItems = satellites.ToDictionary(s => s.Name ?? string.Empty, _ => new List<PlannedScheduleItem>());
        var plannedSchedules = satellites.ToDictionary(s => s.Name ?? string.Empty, _ => new PlannedSchedule());

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

            var plannedScheduleItem = new PlannedScheduleItem()
            {
                TaskName = task?.Name ?? string.Empty,
                Interval = interval,
            };

            plannedScheduleItems[satelliteName].Add(plannedScheduleItem);
        }

        foreach (var item in satellites)
        {
            var satelliteName = item.Name ?? string.Empty;

            var satellitePlannedSchedule = plannedScheduleItems[satelliteName];

            string? Func(IList<ITask>? tasks, string targetName)
            {
                return tasks?
                    .Where(s => s is ObservationTask observationTask && Equals(observationTask.TargetName, targetName))
                    .FirstOrDefault()?.Name;
            }

            var fs = footprints.Select(s => new FootprintFrame()
            {
                TaskName = Func(tasks, s.TargetName ?? string.Empty) ?? string.Empty,
                Center = s.Center ?? new(null),
                Points = s.Points ?? new(null)
            }).ToList();

            plannedSchedules[satelliteName] = new PlannedSchedule()
            {
                Items = plannedScheduleItems[satelliteName],
                Footprints = fs
            };
        }

        return new PlannedScheduleResult()
        {
            Tasks = tasks,
            PlannedSchedules = plannedSchedules
        };
    }
}
