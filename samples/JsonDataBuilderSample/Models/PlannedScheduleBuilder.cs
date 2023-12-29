using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using System.Reactive.Linq;

namespace JsonDataBuilderSample.Models;

public static class PlannedScheduleBuilder
{
    public static async Task<PlannedScheduleObject> CreateAsync(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Observable.Start(() => Create(satellites, groundTargets, groundStations));

    public static PlannedScheduleObject Create(IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var observationTasks = Models.TaskBuilder.CreateObservationTasks(groundTargets);
        var timeWindows = TimeWindowBuilder.Create(satellites, groundTargets);
        var observationTaskResults = Models.TaskResultBuilder.CreateObservations(observationTasks, satellites, timeWindows);
        var observationWindows = Models.TaskAvailabilityBuilder.CreateObservations(observationTasks, satellites, timeWindows);

        return new PlannedScheduleObject()
        {
            Name = "PlannedSchedule1",
            DateTime = DateTime.Now,
            Satellites = satellites.ToList(),
            ObservationTasks = observationTasks,
            GroundTargets = groundTargets.ToList(),
            GroundStations = groundStations.ToList(),
            ObservationWindows = observationWindows,
            ObservationTaskResults = observationTaskResults,
        };
    }
}