using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class ModelFactory
{
    private static async Task<T> Start<T>(Func<T> func)
        => await Observable.Start(func, RxApp.TaskpoolScheduler);

    public static async Task<IList<GroundStation>> CreateDefaultGroundStationsAsync()
        => await Start(() => CreateDefaultGroundStations());

    public static async Task<List<ITask>> CreateTasksAsync(IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Start(() => CreateTasks(groundTargets, groundStations));

    public static async Task<List<ITaskResult>> CreateObservationTaskResultsAsync(IList<ITask> tasks, IList<Footprint> footprints)
        => await Start(() => CreateObservationTaskResults(tasks, footprints));

    public static IList<GroundStation> CreateDefaultGroundStations()
    {
        return new List<GroundStation>()
        {
            new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
        };
    }

    public static List<ITask> CreateTasks(IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var tasks1 = groundTargets
            .Select((s, i) => (ITask)new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = s.Name!
            }).ToList();

        var tasks2 = groundStations
            .Select((s, i) => (ITask)new CommunicationTask()
            {
                Name = $"CommunicationTask{(i + 1):0000}",
                GroundStationName = s.Name!
            }).ToList();

        return tasks1.Concat(tasks2).ToList();
    }

    public static List<ITaskResult> CreateObservationTaskResults(IList<ITask> tasks, IList<Footprint> footprints)
    {
        return tasks
            .Where(s => s is ObservationTask)
            .Cast<ObservationTask>()
            .SelectMany(s =>
                footprints
                    .Where(f => Equals(f.TargetName, s.GroundTargetName))
                    .Select(f => CreateObservationTaskResult(s.Name, f)))
            .ToList();
    }

    public static ITaskResult CreateObservationTaskResult(string taskName, Footprint footprint)
    {
        var begin = footprint.Begin;
        var duration = footprint.Duration;

        var taskResult = new ObservationTaskResult()
        {
            TaskName = taskName,
            SatelliteName = footprint.SatelliteName ?? "SatelliteDefault",
            Interval = new Interval { Begin = begin, Duration = duration },
            Footprint = new FootprintFrame { Center = footprint.Center!, Points = footprint.Points! },
            Transition = null
        };

        return taskResult;
    }
}
