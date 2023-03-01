using FootprintViewer.Data.Models;
using RandomDataBuilder.Sources;

namespace JsonDataBuilderSample;

public static class DataBuilder
{
    public static async Task<IList<Footprint>> CreateRandomFootprints(IList<Satellite> satellites, int count)
    {
        var footprintSource = new FootprintRandomSource()
        {
            GenerateCount = count,
            Satellites = satellites
        };

        var res = await footprintSource.GetValuesAsync();

        return res.Cast<Footprint>().ToList();
    }

    public static async Task<IList<GroundTarget>> CreateRandomGroundTargets(IList<Footprint> footprints, int count)
    {
        var groundTargetsSource = new GroundTargetRandomSource()
        {
            GenerateCount = count,
            Footprints = footprints
        };

        var res = await groundTargetsSource.GetValuesAsync();

        return res.Cast<GroundTarget>().ToList();
    }

    public static async Task<IList<Satellite>> CreateRandomSatellites(int count)
    {
        var satellitesSource = new SatelliteRandomSource()
        {
            GenerateCount = count
        };

        var res = await satellitesSource.GetValuesAsync();

        return res.Cast<Satellite>().ToList();
    }

    public static async Task<IList<GroundStation>> CreateRandomGroundStations(int count)
    {
        var groundStationsSource = new GroundStationRandomSource()
        {
            GenerateCount = (count > 6) ? 6 : count
        };

        var res = await groundStationsSource.GetValuesAsync();

        return res.Cast<GroundStation>().ToList();
    }
}
