using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using ReactiveUI;
using SpaceScience;
using SpaceScience.Extensions;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class PlannedScheduleBuilder
{
    public static async Task<PlannedScheduleResult> CreateAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
        => await Observable.Start(() => Create(satellites, groundTargets, groundStations, footprints), RxApp.TaskpoolScheduler);

    public static async Task<PlannedScheduleResult> CreateAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Observable.Start(() => Create(satellites, groundTargets, groundStations), RxApp.TaskpoolScheduler);

    public static PlannedScheduleResult Create(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        var tasks = TaskBuilder.Create(groundTargets, groundStations);

        var observationTasks = TaskResultBuilder.CreateObservations(tasks, footprints);
        var observationWindows = TaskAvailabilityBuilder.CreateObservations(footprints, satellites, tasks);
        var communicationWindows = TaskAvailabilityBuilder.CreateCommunications(satellites, groundStations, footprints, tasks);
        var communicationTasks = TaskResultBuilder.CreateCommunications(communicationWindows);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Satellites = satellites.ToList(),
            GroundTargets = groundTargets.ToList(),
            Tasks = tasks,
            TaskAvailabilities = observationWindows.Concat(communicationWindows).ToList(),
            PlannedSchedules = observationTasks.Concat(communicationTasks).ToList()
        };
    }

    public static PlannedScheduleResult Create(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var tasks = TaskBuilder.Create(groundTargets, groundStations);

        var timeWindows = CreateTimeWindows(satellites, groundTargets);
        var observationTasks = TaskResultBuilder.CreateObservations(tasks, satellites, timeWindows);
        var observationWindows = TaskAvailabilityBuilder.CreateObservations(tasks, satellites, timeWindows);
        // var communicationWindows = TaskAvailabilityBuilder.CreateCommunications(satellites, groundStations, footprints, tasks);
        // var communicationTasks = TaskResultBuilder.CreateCommunications(communicationWindows);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Satellites = satellites.ToList(),
            Tasks = tasks,
            GroundTargets = groundTargets.ToList(),
            // TaskAvailabilities = observationWindows.Concat(communicationWindows).ToList(),
            TaskAvailabilities = observationWindows.ToList(),
            //  PlannedSchedules = observationTasks.Concat(communicationTasks).ToList()
            PlannedSchedules = observationTasks.ToList()
        };
    }

    private static IList<(string satName, IList<TimeWindowResult> windows)> CreateTimeWindows(IList<Satellite> satellites, IList<GroundTarget> gts)
    {
        var targets = gts.Select(s => ToTarget(s)).ToList();

        var list = new List<(string satName, IList<TimeWindowResult> windows)>();

        foreach (var sat in satellites)
        {
            var orbit = sat.ToOrbit();
            var nodes = orbit.NodesOnDay();
            var gam1Deg = sat.LookAngleDeg - sat.RadarAngleDeg / 2.0;
            var gam2Deg = sat.LookAngleDeg + sat.RadarAngleDeg / 2.0;

            var res = SpaceMethods.ObservationGroundTargets(orbit, 0, nodes - 1, gam1Deg, gam2Deg, targets);

            list.Add((sat.Name!, res));
        }

        return list;

        static (double lonDeg, double latDeg, string name) ToTarget(GroundTarget gt)
        {
            var coord = gt.Points?.Centroid.Coordinate ?? new();
            var name = gt.Name ?? "";
            return (coord.X, coord.Y, name);
        }
    }

    public static PlannedScheduleResult CreateRandom()
    {
        var count = 10;

        var satellites = SatelliteBuilder.Create(1);
        var footprints = FootprintBuilder.Create(satellites, count);
        var gts = GroundTargetBuilder.Create(footprints, count);

        var tasks = TaskBuilder.Create(gts, new List<GroundStation>());
        var observationTasks = TaskResultBuilder.CreateObservations(tasks, footprints);
        var windows = TaskAvailabilityBuilder.CreateObservations(footprints, satellites, tasks);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule01",
            DateTime = DateTime.Now,
            Satellites = satellites.ToList(),
            GroundTargets = gts.ToList(),
            Tasks = tasks,
            TaskAvailabilities = windows,
            PlannedSchedules = observationTasks
        };
    }
}
