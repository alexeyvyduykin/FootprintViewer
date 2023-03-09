using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using ReactiveUI;
using SkiaSharp;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class RandomModelBuilder
{
    private static readonly Random _random = new();

    private static async Task<T> Start<T>(Func<T> func)
        => await Observable.Start(func, RxApp.TaskpoolScheduler);

    public static async Task<IList<Satellite>> CreateSatellitesAsync(int count)
        => await Start(() => CreateSatellites(count));

    public static IList<Satellite> CreateSatellites(int count)
        => DefaultSatelliteBuilder.Create(count);

    public static async Task<IList<Footprint>> CreateFootprintsAsync(IList<Satellite> satellites, int count)
        => await Start(() => CreateFootprints(satellites, count));

    public static IList<Footprint> CreateFootprints(IList<Satellite> satellites, int count)
        => FootprintBuilder.Create(satellites, count);

    public static async Task<IList<GroundTarget>> CreateGroundTargetsAsync(IList<Footprint> footprints, int count)
        => await Start(() => CreateGroundTargets(footprints, count));

    public static IList<GroundTarget> CreateGroundTargets(IList<Footprint> footprints, int count)
        => GroundTargetBuilder.Create(footprints, count);

    public static async Task<List<ITaskResult>> CreateCommunicationTaskResultsAsync(List<TaskAvailability> availabilities)
        => await Start(() => CreateCommunicationTaskResults(availabilities));

    public static async Task<List<TaskAvailability>> CreateCommunicationTaskAvailabilitiesAsync(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
        => await Start(() => CreateCommunicationTaskAvailabilities(satellites, groundStations, footprints, tasks));

    public static async Task<List<TaskAvailability>> CreateObservationTaskAvailabilitiesAsync(IList<Footprint> footprints, IList<Satellite> satellites, IList<ITask> tasks)
        => await Start(() => CreateObservationTaskAvailabilities(footprints, satellites, tasks));

    public static async Task<PlannedScheduleResult> CreatePlannedScheduleAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
        => await Start(() => CreatePlannedSchedule(satellites, groundTargets, groundStations, footprints));

    public static FootprintPreview BuildFootprintPreview()
    {
        var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
        var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

        var name = names[_random.Next(0, names.Length)].Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
        var date = DateTime.UtcNow;

        var unitBitmap = new SKBitmap(1, 1);
        unitBitmap.SetPixel(0, 0, SKColors.White);

        return new FootprintPreview()
        {
            Name = name.ToUpper(),
            Date = date.Date.ToShortDateString(),
            SatelliteName = satellites[_random.Next(0, satellites.Length)],
            SunElevation = _random.Next(0, 91),
            CloudCoverFull = _random.Next(0, 101),
            TileNumber = name.ToUpper(),
            Image = SKImage.FromBitmap(unitBitmap)
        };
    }

    public static Footprint BuildFootprint()
    {
        return new Footprint()
        {
            Name = $"Footprint{_random.Next(1, 101):000}",
            SatelliteName = $"Satellite{_random.Next(1, 10):00}",
            Center = new Point(_random.Next(-180, 180), _random.Next(-90, 90)),
            Begin = DateTime.Now,
            Duration = _random.Next(20, 40),
            Node = _random.Next(1, 16),
            Direction = (SwathDirection)_random.Next(0, 2),
        };
    }

    public static GroundStation BuildGroundStation()
    {
        return new GroundStation()
        {
            Name = $"London",
            Center = new Point(-0.118092, 51.509865),
            Angles = new double[] { 12, 18, 22, 26, 30 },
        };
    }

    public static GroundTarget BuildGroundTarget()
    {
        var type = (GroundTargetType)_random.Next(0, 3);

        return new GroundTarget()
        {
            Name = $"GroundTarget{_random.Next(1, 101):000}",
            Type = type,
        };
    }

    public static Satellite BuildSatellite()
    {
        return new Satellite()
        {
            Name = $"Satellite{_random.Next(1, 10):00}",
            Semiaxis = 6945.03,
            Eccentricity = 0.0,
            InclinationDeg = 97.65,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = 0.0,
            RightAscensionAscendingNodeDeg = 0.0,
            Period = 5760.0,
            Epoch = DateTime.Now,
            InnerHalfAngleDeg = 32,
            OuterHalfAngleDeg = 48
        };
    }

    public static UserGeometry BuildUserGeometry()
    {
        return new UserGeometry()
        {
            Name = $"UserGeometry{_random.Next(1, 101):000}",
            Type = (UserGeometryType)_random.Next(0, 4),
        };
    }

    public static PlannedScheduleResult BuildPlannedSchedule()
    {
        var count = 10;

        var satellites = RandomModelBuilder.CreateSatellites(1);
        var footprints = RandomModelBuilder.CreateFootprints(satellites, count);
        var gts = RandomModelBuilder.CreateGroundTargets(footprints, count);

        var tasks = ModelFactory.CreateTasks(gts, new List<GroundStation>());
        var observationTasks = ModelFactory.CreateObservationTaskResults(tasks, footprints);
        var windows = RandomModelBuilder.CreateObservationTaskAvailabilities(footprints, satellites, tasks);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule01",
            DateTime = DateTime.Now,
            Tasks = tasks,
            TaskAvailabilities = windows,
            PlannedSchedules = observationTasks
        };
    }

    public static List<ITaskResult> CreateCommunicationTaskResults(List<TaskAvailability> availabilities)
    {
        var taskResults = new List<ITaskResult>();

        foreach (var item in availabilities)
        {
            var count = item.Windows.Count;

            var indexUplink = _random.Next(0, count);
            var indexDownlink = _random.Next(0, count);
            if (indexUplink == indexDownlink)
            {
                indexDownlink++;
            }

            for (int i = 0; i < count; i++)
            {
                var ival = item.Windows[i];

                if (i == indexUplink)
                {
                    var begin = ival.Begin;
                    var duration = ival.Duration;
                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
                    var start = _random.Next(0, (int)(duration - newDuration));

                    var taskResult = new CommunicationTaskResult()
                    {
                        TaskName = item.TaskName,
                        SatelliteName = item.SatelliteName,
                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
                        Type = CommunicationType.Uplink,
                    };

                    taskResults.Add(taskResult);
                }

                if (i == indexDownlink)
                {
                    var begin = ival.Begin;
                    var duration = ival.Duration;
                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
                    var start = _random.Next(0, (int)(duration - newDuration));

                    var taskResult = new CommunicationTaskResult()
                    {
                        TaskName = item.TaskName,
                        SatelliteName = item.SatelliteName,
                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
                        Type = CommunicationType.Downlink,
                    };

                    taskResults.Add(taskResult);
                }
            }
        }

        return taskResults;
    }

    public static List<TaskAvailability> CreateCommunicationTaskAvailabilities(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    {
        var radius = 10.0;
        var minTaskAvailability = 120;
        var maxTaskAvailability = 181;

        var list = new List<TaskAvailability>();

        var commiunicationTasks = tasks.Where(s => s is CommunicationTask).Cast<CommunicationTask>().ToList();

        foreach (var satName in satellites.Select(s => s.Name ?? "SatelliteDefault"))
        {
            foreach (var item in commiunicationTasks)
            {
                var gsName = item.GroundStationName;
                var gs = groundStations.Where(s => Equals(s.Name, gsName)).Single();

                var visibleIntervals = footprints
                    .Where(s => IsInArea(s.Center!, gs.Center, radius))
                    .Select(s => (s.Begin, s.Duration))
                    .ToList();

                var newIntervals = new List<Interval>();

                foreach (var (begin, duration) in visibleIntervals)
                {
                    var centerDateTime = begin.AddSeconds(duration / 2.0);
                    var newDuration = _random.Next(minTaskAvailability, maxTaskAvailability);
                    var newHalfDuration = newDuration / 2.0;

                    var newBegin = centerDateTime.AddSeconds(-newHalfDuration);

                    newIntervals.Add(new Interval() { Begin = newBegin, Duration = newDuration });
                }

                var validIntervals = ToValidRange(newIntervals);

                var res = new TaskAvailability()
                {
                    TaskName = item.Name,
                    SatelliteName = satName,
                    Windows = validIntervals
                };

                list.Add(res);
            }
        }

        return list;
    }

    public static List<TaskAvailability> CreateObservationTaskAvailabilities(IList<Footprint> footprints, IList<Satellite> satellites, IList<ITask> tasks)
    {
        var minTaskAvailability = 60;
        var maxTaskAvailability = 121;

        var observationTasks = tasks.Where(s => s is ObservationTask).Cast<ObservationTask>().ToList();

        var list = new List<TaskAvailability>();

        foreach (var item in footprints)
        {
            var begin = item.Begin;
            var duration = item.Duration;

            var gtName = item.TargetName;
            var interval = new Interval()
            {
                Begin = item.Begin,
                Duration = item.Duration
            };

            var task = observationTasks
                .Where(s => Equals(s.GroundTargetName, gtName))
                .FirstOrDefault()!;

            var centerDateTime = begin.AddSeconds(duration / 2.0);

            var arr = new List<TaskAvailability>();

            foreach (var satName in satellites.Select(s => s.Name))
            {
                var newDuration = _random.Next(minTaskAvailability, maxTaskAvailability);
                var newHalfDuration = newDuration / 2.0;
                var newBegin = centerDateTime.AddSeconds(-newHalfDuration);

                var ival = new Interval { Begin = newBegin, Duration = newDuration };

                var res = new TaskAvailability()
                {
                    TaskName = task.Name,
                    SatelliteName = satName!,
                    Windows = new() { ival }
                };

                arr.Add(res);
            }

            list.AddRange(arr);
        }

        return list;
    }

    private static bool IsInArea(Point point, Point center, double r)
    {
        var c0 = center.Coordinate;
        var c1 = new Coordinate(c0.X - r, c0.Y + r);
        var c2 = new Coordinate(c0.X + r, c0.Y + r);
        var c3 = new Coordinate(c0.X + r, c0.Y - r);
        var c4 = new Coordinate(c0.X - r, c0.Y - r);
        var poly = new Polygon(new LinearRing(new[] { c1, c2, c3, c4, c1 }));
        return poly.Contains(point);
    }

    private static List<Interval> ToValidRange(List<Interval> intervals)
    {
        var list = new List<Interval>();

        foreach (var item in intervals)
        {
            if (list.Count == 0)
            {
                list.Add(item);
            }
            else
            {
                var current = list.Last();

                if (current.End() > item.Begin)
                {
                    var min = current.Begin > item.Begin ? item.Begin : current.Begin;
                    var max = current.End() > item.End() ? current.End() : item.End();

                    list.RemoveAt(list.Count - 1);
                    list.Add(new() { Begin = min, Duration = (max - min).TotalSeconds });
                }
                else
                {
                    list.Add(item);
                }
            }
        }

        return list;
    }

    private static DateTime End(this Interval interval)
    {
        return interval.Begin.AddSeconds(interval.Duration);
    }

    public static PlannedScheduleResult CreatePlannedSchedule(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        var tasks = ModelFactory.CreateTasks(groundTargets, groundStations);

        var observationTasks = ModelFactory.CreateObservationTaskResults(tasks, footprints);
        var observationWindows = RandomModelBuilder.CreateObservationTaskAvailabilities(footprints, satellites, tasks);
        var communicationWindows = RandomModelBuilder.CreateCommunicationTaskAvailabilities(satellites, groundStations, footprints, tasks);
        var communicationTasks = RandomModelBuilder.CreateCommunicationTaskResults(communicationWindows);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Tasks = tasks,
            TaskAvailabilities = observationWindows.Concat(communicationWindows).ToList(),
            PlannedSchedules = observationTasks.Concat(communicationTasks).ToList()
        };
    }
}
