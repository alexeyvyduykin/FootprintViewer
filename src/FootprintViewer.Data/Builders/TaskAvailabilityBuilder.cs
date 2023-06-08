using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SpaceScience;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class TaskAvailabilityBuilder
{
    private static readonly Random _random = new();

    public static async Task<List<TaskAvailability>> CreateCommunicationsAsync(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    => await Observable.Start(() => CreateCommunications(satellites, groundStations, footprints, tasks));

    public static async Task<List<TaskAvailability>> CreateObservationsAsync(IList<Footprint> footprints, IList<Satellite> satellites, IList<ITask> tasks)
        => await Observable.Start(() => CreateObservations(footprints, satellites, tasks));

    public static List<TaskAvailability> CreateObservations(IList<ITask> tasks, IList<Satellite> satellites, IList<(string satName, IList<TimeWindowResult> windows)> windows)
    {
        var list = new List<TaskAvailability>();

        foreach (var task in tasks.Where(s => s is ObservationTask).Cast<ObservationTask>())
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

    public static List<TaskAvailability> CreateCommunications(IList<Satellite> satellites, IList<GroundStation> groundStations, IList<Footprint> footprints, IList<ITask> tasks)
    {
        var radius = 10.0;
        var minTaskAvailability = 120;
        var maxTaskAvailability = 181;

        var list = new List<TaskAvailability>();

        var communicationTasks = tasks.Where(s => s is CommunicationTask).Cast<CommunicationTask>().ToList();

        foreach (var satName in satellites.Select(s => s.Name ?? "SatelliteDefault"))
        {
            foreach (var item in communicationTasks)
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

                var res = new TaskAvailability()
                {
                    TaskName = item.Name,
                    SatelliteName = satName,
                    Windows = newIntervals.Merge()
                };

                list.Add(res);
            }
        }

        return list;
    }

    public static List<TaskAvailability> CreateObservations(IList<Footprint> footprints, IList<Satellite> satellites, IList<ITask> tasks)
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
}
