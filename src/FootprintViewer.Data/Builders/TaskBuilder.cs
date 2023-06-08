using FootprintViewer.Data.Models;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class TaskBuilder
{
    public static async Task<List<ITask>> CreateAsync(IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Observable.Start(() => Create(groundTargets, groundStations));

    public static List<ITask> Create(IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var list = new List<ITask>();

        foreach (var (gt, i) in groundTargets.Select((s, index) => (s, index)))
        {
            list.Add(new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = gt.Name!
            });
        }

        foreach (var (gs, i) in groundStations.Select((s, index) => (s, index)))
        {
            list.Add(new CommunicationTask()
            {
                Name = $"CommunicationTask{(i + 1):0000}",
                GroundStationName = gs.Name!
            });
        }

        return list;
    }
}
