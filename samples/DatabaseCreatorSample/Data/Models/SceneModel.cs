using FootprintViewer.Data;
using RandomDataBuilder.Sources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseCreatorSample.Data;

public sealed class SceneModel
{
    private SceneModel() { }

    public List<Satellite> Satellites { get; set; } = new();

    public List<GroundTarget> GroundTargets { get; set; } = new();

    public List<Footprint> Footprints { get; set; } = new();

    public List<GroundStation> GroundStations { get; set; } = new();

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

    public static async Task<SceneModel> BuildAsync()
    {
        var sceneModel = new SceneModel()
        {
            Satellites = new List<Satellite>(),
            GroundTargets = new List<GroundTarget>(),
            Footprints = new List<Footprint>(),
            GroundStations = new List<GroundStation>(),
        };

        var satellites = await CreateRandomSatellites(5);
        var gss = await CreateRandomGroundStations(6);
        var footprints = await CreateRandomFootprints(satellites, 2000);
        var gts = await CreateRandomGroundTargets(footprints, 5000);

        sceneModel.Satellites.AddRange(satellites);
        sceneModel.Footprints.AddRange(footprints);
        sceneModel.GroundTargets.AddRange(gts);
        sceneModel.GroundStations.AddRange(gss);

        return sceneModel;
    }
}
