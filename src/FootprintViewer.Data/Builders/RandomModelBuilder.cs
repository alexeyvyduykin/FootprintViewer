using FootprintViewer.Data.Models;
using FootprintViewer.Data.RandomSources;
using NetTopologySuite.Geometries;
using ReactiveUI;
using SkiaSharp;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class RandomModelBuilder
{
    private static readonly Random _random = new();

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

        var footprints = Enumerable.Range(0, count).Select(_ => BuildFootprint()).ToList();

        var tasks = footprints.Select((s, i) => (ITask)new ObservationTask() { Name = $"ObservationTask{i + 1}", GroundTargetName = s.TargetName! }).ToList();

        var list = tasks.Select((s, i) => BuildObservationTaskResult(s.Name, footprints[i])).ToList();
        var windows = tasks.Select((s, i) => BuildTaskAvailabilities(s.Name, footprints[i])).ToList();

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule01",
            DateTime = DateTime.Now,
            Tasks = tasks,
            TaskAvailabilities = windows,
            PlannedSchedules = list
        };
    }

    public static List<ITaskResult> BuildObservationTaskResults(IList<ITask> tasks, IList<Footprint>? footprints)
    {
        if (footprints == null)
        {
            return new();
        }

        return tasks
            .Where(s => s is ObservationTask)
            .Cast<ObservationTask>()
            .SelectMany(s =>
                footprints
                    .Where(f => Equals(f.TargetName, s.GroundTargetName))
                    .Select(f => BuildObservationTaskResult(s.Name, f)))
            .ToList();

        //foreach (var item in tasks.Where(s => s is ObservationTask).Cast<ObservationTask>())
        //{
        //    var list = footprints.Where(s => Equals(s.TargetName, item.TargetName)).ToList();

        //    foreach (var footprint in list)
        //    {
        //        var satelliteName = footprint.SatelliteName!;

        //        var task = CreateObservationTaskResult(item.Name, footprint);

        //        result.Add(task);
        //    }
        //}

        //return result;
    }

    public static async Task<List<ITaskResult>> BuildObservationTaskResultsAsync(IList<ITask> tasks, IList<Footprint> footprints)
    {
        return await Observable.Start(() => BuildObservationTaskResults(tasks, footprints), RxApp.TaskpoolScheduler);
    }

    public static List<ITask> BuildTasks(IList<GroundTarget>? groundTargets)
    {
        return groundTargets?
            .Select((s, i) => (ITask)new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = s.Name!
            }).ToList() ?? new();
    }

    public static async Task<List<ITask>> BuildTasksAsync(IList<GroundTarget> groundTargets)
    {
        return await Observable.Start(() => BuildTasks(groundTargets), RxApp.TaskpoolScheduler);
    }

    public static ITaskResult BuildObservationTaskResult(string taskName, Footprint footprint)
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

    private static TaskAvailability BuildTaskAvailabilities(string taskName, Footprint footprint)
    {
        var begin = footprint.Begin;
        var duration = footprint.Duration;

        var windowDuration = duration * (_random.Next(30, 51) / 10.0);

        var windowBeginSec = _random.Next(0, (int)(windowDuration - duration) + 1);

        var windowBegin = begin.AddSeconds(-windowBeginSec);

        return new TaskAvailability()
        {
            TaskName = taskName,
            SatelliteName = footprint.SatelliteName ?? "SatelliteDefault",
            Windows = new[] { new Interval { Begin = windowBegin, Duration = windowDuration } }.ToList()
        };
    }

    public static List<ITaskResult> BuildCommunicationTaskResults(List<TaskAvailability> availabilities)
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

    public static List<TaskAvailability> BuildCommunicationTaskAvailabilities(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
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

    public static List<TaskAvailability> BuildObservationTaskAvailabilities(IList<Footprint> footprints, IList<ITask> tasks)
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

            var centerDateTime = begin.AddSeconds(duration / 2.0);
            var newDuration = _random.Next(minTaskAvailability, maxTaskAvailability);
            var newHalfDuration = newDuration / 2.0;

            var newBegin = centerDateTime.AddSeconds(-newHalfDuration);

            var ival = new Interval { Begin = newBegin, Duration = newDuration };

            var task = observationTasks
                .Where(s => Equals(s.GroundTargetName, gtName))
                .FirstOrDefault()!;

            var res = new TaskAvailability()
            {
                TaskName = task.Name,
                SatelliteName = item.SatelliteName!,
                Windows = new() { ival }
            };

            list.Add(res);
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

    //--------------------------------------------------------------

    public static async Task<IList<Footprint>> BuildRandomFootprintsAsync(IList<Satellite> satellites, int count)
    {
        var footprintSource = new FootprintRandomSource()
        {
            GenerateCount = count,
            Satellites = satellites
        };

        var res = await footprintSource.GetValuesAsync();

        return res.Cast<Footprint>().ToList();
    }

    public static async Task<IList<GroundTarget>> BuildRandomGroundTargetsAsync(IList<Footprint> footprints, int count)
    {
        var groundTargetsSource = new GroundTargetRandomSource()
        {
            GenerateCount = count,
            Footprints = footprints
        };

        var res = await groundTargetsSource.GetValuesAsync();

        return res.Cast<GroundTarget>().ToList();
    }

    public static async Task<IList<Satellite>> BuildRandomSatellitesAsync(int count)
    {
        var satellitesSource = new SatelliteRandomSource()
        {
            GenerateCount = count
        };

        var res = await satellitesSource.GetValuesAsync();

        return res.Cast<Satellite>().ToList();
    }

    public static async Task<IList<GroundStation>> BuildRandomGroundStationsAsync(int count)
    {
        var groundStationsSource = new GroundStationRandomSource()
        {
            GenerateCount = (count > 6) ? 6 : count
        };

        var res = await groundStationsSource.GetValuesAsync();

        return res.Cast<GroundStation>().ToList();
    }
}
