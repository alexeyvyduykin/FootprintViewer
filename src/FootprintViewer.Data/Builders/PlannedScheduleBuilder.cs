﻿using FootprintViewer.Data.Models;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class PlannedScheduleBuilder
{
    public static async Task<PlannedScheduleResult> CreateAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
        => await Observable.Start(() => Create(satellites, groundTargets, groundStations, footprints));

    public static async Task<PlannedScheduleResult> CreateAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Observable.Start(() => Create(satellites, groundTargets, groundStations));

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
            GroundStations = groundStations.ToList(),
            Tasks = tasks,
            TaskAvailabilities = observationWindows.Concat(communicationWindows).ToList(),
            PlannedSchedules = observationTasks.Concat(communicationTasks).ToList()
        };
    }

    public static PlannedScheduleResult Create(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var tasks = TaskBuilder.Create(groundTargets, groundStations);

        var timeWindows = TimeWindowBuilder.Create(satellites, groundTargets);
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
            GroundStations = groundStations.ToList(),
            // TaskAvailabilities = observationWindows.Concat(communicationWindows).ToList(),
            TaskAvailabilities = observationWindows.ToList(),
            //  PlannedSchedules = observationTasks.Concat(communicationTasks).ToList()
            PlannedSchedules = observationTasks.ToList()
        };
    }

    public static PlannedScheduleResult CreateRandom()
    {
        var count = 100;

        var satellites = SatelliteBuilder.Create(5);
        var footprints = FootprintBuilder.Create(satellites, count);
        var gts = GroundTargetBuilder.Create(footprints, count);
        var gss = GroundStationBuilder.CreateDefault();

        var tasks = TaskBuilder.Create(gts, gss);
        var observationTasks = TaskResultBuilder.CreateObservations(tasks, footprints);
        var windows = TaskAvailabilityBuilder.CreateObservations(footprints, satellites, tasks);

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule01",
            DateTime = DateTime.Now,
            Satellites = satellites.ToList(),
            GroundTargets = gts.ToList(),
            GroundStations = gss.ToList(),
            Tasks = tasks,
            TaskAvailabilities = windows,
            PlannedSchedules = observationTasks
        };
    }
}
