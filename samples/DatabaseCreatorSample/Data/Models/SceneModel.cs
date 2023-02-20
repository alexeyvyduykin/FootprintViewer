using DatabaseCreatorSample.Data.Models;
using FootprintViewer.Data;
using RandomDataBuilder.Sources;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DatabaseCreatorSample.Data;

public sealed class SceneModel
{
    private SceneModel() { }

    public List<Satellite> Satellites { get; set; } = new();

    public List<GroundTarget> GroundTargets { get; set; } = new();

    public List<Footprint> Footprints { get; set; } = new();

    public List<GroundStation> GroundStations { get; set; } = new();

    public List<ObservationTask> ObservationTasks { get; set; } = new();

    public List<CommunicationTask> CommunicationTasks { get; set; } = new();

    public List<TransitionTask> TransitionTasks { get; set; } = new();

    private static async Task<IList<Footprint>> CreateRandomFootprints(IList<Satellite> satellites, int count)
    {
        var footprintSource = new FootprintRandomSource()
        {
            GenerateCount = count,
            Satellites = satellites
        };

        var res = await footprintSource.GetValuesAsync();

        return res.Cast<Footprint>().ToList();
    }

    private static async Task<IList<GroundTarget>> CreateRandomGroundTargets(IList<Footprint> footprints, int count)
    {
        var groundTargetsSource = new GroundTargetRandomSource()
        {
            GenerateCount = count,
            Footprints = footprints
        };

        var res = await groundTargetsSource.GetValuesAsync();

        return res.Cast<GroundTarget>().ToList();
    }

    private static async Task<IList<Satellite>> CreateRandomSatellites(int count)
    {
        var satellitesSource = new SatelliteRandomSource()
        {
            GenerateCount = count
        };

        var res = await satellitesSource.GetValuesAsync();

        return res.Cast<Satellite>().ToList();
    }

    private static async Task<IList<GroundStation>> CreateRandomGroundStations(int count)
    {
        var groundStationsSource = new GroundStationRandomSource()
        {
            GenerateCount = (count > 6) ? 6 : count
        };

        var res = await groundStationsSource.GetValuesAsync();

        return res.Cast<GroundStation>().ToList();
    }

    private static async Task<IList<ObservationTask>> CreateObservationTasks(IList<Footprint> footprints)
    {
        return await Observable.Start(() => ObservationTaskBuilder.Create(footprints), RxApp.TaskpoolScheduler);
    }

    private static async Task<IList<CommunicationTask>> CreateCommunicationTasks()
    {
        return await Observable.Start(() => new List<CommunicationTask>(), RxApp.TaskpoolScheduler);
    }

    private static async Task<IList<TransitionTask>> CreateTransitionTasks()
    {
        return await Observable.Start(() => new List<TransitionTask>(), RxApp.TaskpoolScheduler);
    }

    public static async Task<SceneModel> BuildAsync()
    {
        var sceneModel = new SceneModel();

        var satellites = await CreateRandomSatellites(5);
        var gss = await CreateRandomGroundStations(6);
        var footprints = await CreateRandomFootprints(satellites, 2000);
        var gts = await CreateRandomGroundTargets(footprints, 5000);

        var observationTasks = await CreateObservationTasks(footprints);
        var communicationTasks = await CreateCommunicationTasks();
        var transitionTasks = await CreateTransitionTasks();

        sceneModel.Satellites.AddRange(satellites);
        sceneModel.Footprints.AddRange(footprints);
        sceneModel.GroundTargets.AddRange(gts);
        sceneModel.GroundStations.AddRange(gss);

        sceneModel.ObservationTasks.AddRange(observationTasks);
        sceneModel.CommunicationTasks.AddRange(communicationTasks);
        sceneModel.TransitionTasks.AddRange(transitionTasks);

        return sceneModel;
    }
}
