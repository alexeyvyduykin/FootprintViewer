using FootprintViewer.Data.Models;
using System.Reactive.Linq;

namespace JsonDataBuilderSample.Models;

public static class TaskBuilder
{
    public static List<ObservationTask> CreateObservationTasks(IList<GroundTarget> groundTargets)
    {
        var list = new List<ObservationTask>();

        foreach (var (gt, i) in groundTargets.Select((s, index) => (s, index)))
        {
            list.Add(new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                GroundTargetName = gt.Name!
            });
        }

        return list;
    }

    public static List<CommunicationTask> CreateCommunicationTasks(IList<GroundStation> groundStations)
    {
        var list = new List<CommunicationTask>();

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
