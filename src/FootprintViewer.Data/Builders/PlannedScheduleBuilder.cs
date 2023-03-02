using FootprintViewer.Data.Models;
using ReactiveUI;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class PlannedScheduleBuilder
{
    public static PlannedScheduleResult Build(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        var observationTasks = groundTargets.Select((s, index) => new ObservationTask()
        {
            Name = $"ObservationTask_{index + 1}",
            GroundTargetName = s.Name ?? string.Empty
        }).OfType<ITask>().ToList();

        var communicationTasks = groundStations.Select((s, index) => new CommunicationTask()
        {
            Name = $"CommunicationTask_{index + 1}",
            GroundStationName = s.Name ?? string.Empty
        }).OfType<ITask>().ToList();

        var tasks = observationTasks.Concat(communicationTasks).ToList();

        var observationTaskResults = BuildObservationTaskResults(footprints, tasks);
        var observationTaskAvailabilities = RandomModelBuilder.BuildObservationTaskAvailabilities(footprints, tasks);
        var comminicationTaskAvailabilities = RandomModelBuilder.BuildCommunicationTaskAvailabilities(satellites, groundStations, footprints, tasks);
        var comminicationTaskResults = RandomModelBuilder.BuildCommunicationTaskResults(comminicationTaskAvailabilities);

        var list = observationTaskResults.Concat(comminicationTaskResults).ToList();
        var taskAvailabilities = observationTaskAvailabilities.Concat(comminicationTaskAvailabilities).ToList();

        return new PlannedScheduleResult()
        {
            Tasks = tasks,
            TaskAvailabilities = taskAvailabilities,
            PlannedSchedules = list
        };
    }

    public static async Task<PlannedScheduleResult> BuildAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        return await Observable.Start(() => Build(satellites, groundTargets, groundStations, footprints), RxApp.TaskpoolScheduler);
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
                .Where(s => s is ObservationTask observationTask && Equals(observationTask.GroundTargetName, targetName))
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

    private static List<ITaskResult> BuildCommunicationTaskResults22(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
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
                    .Where(s => s is CommunicationTask communicationTask && Equals(communicationTask.GroundStationName, gs.Name))
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

    //private static List<ITaskResult> BuildCommunicationTaskResults2(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    //{
    //    var radius = 10.0;

    //    var taskResults = new List<ITaskResult>();

    //    var commiunicationTasks = tasks.Where(s => s is CommunicationTask).Cast<CommunicationTask>().ToList();

    //    foreach (var satName in satellites.Select(s => s.Name ?? "SatelliteDefault"))
    //    {

    //        foreach (var item in commiunicationTasks)
    //        {
    //            var gsName = item.GroundStationName;
    //            var gs = groundStations.Where(s => Equals(s.Name, gsName)).Single();

    //            var visibleIntervals = footprints
    //                .Where(s => IsInArea(s.Center!, gs.Center, radius))
    //                .Select(s => (s.Begin, s.Duration))
    //                .ToList();

    //            var newIntervals = new List<Interval>();

    //            foreach (var (begin, duration) in visibleIntervals)
    //            {
    //                var centerDateTime = begin.AddSeconds(duration / 2.0);
    //                var newDuration = _random.Next(120, 181);
    //                var newHalfDuration = newDuration / 2.0;

    //                var newBegin = centerDateTime.AddSeconds(-newHalfDuration);

    //                newIntervals.Add(new Interval() { Begin = newBegin, Duration = newDuration });
    //            }

    //            var validIntervals = ToValidRange(newIntervals);

    //            var count = validIntervals.Count;

    //            var indexUplink = _random.Next(0, count);
    //            var indexDownlink = _random.Next(0, count);
    //            if (indexUplink == indexDownlink)
    //            {
    //                indexDownlink++;
    //            }

    //            for (int i = 0; i < count; i++)
    //            {
    //                var ival = validIntervals[i];

    //                if (i == indexUplink)
    //                {
    //                    var begin = ival.Begin;
    //                    var duration = ival.Duration;
    //                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
    //                    var start = _random.Next(0, (int)(duration - newDuration));

    //                    var taskResult = new CommunicationTaskResult()
    //                    {
    //                        TaskName = item.Name,
    //                        SatelliteName = satName,
    //                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
    //                        Windows = validIntervals,
    //                        Type = CommunicationType.Uplink,
    //                    };

    //                    taskResults.Add(taskResult);
    //                }

    //                if (i == indexDownlink)
    //                {
    //                    var begin = ival.Begin;
    //                    var duration = ival.Duration;
    //                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
    //                    var start = _random.Next(0, (int)(duration - newDuration));

    //                    var taskResult = new CommunicationTaskResult()
    //                    {
    //                        TaskName = item.Name,
    //                        SatelliteName = satName,
    //                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
    //                        Windows = validIntervals,
    //                        Type = CommunicationType.Downlink,
    //                    };

    //                    taskResults.Add(taskResult);
    //                }

    //            }
    //        }
    //    }

    //    return taskResults;
    //}
}
