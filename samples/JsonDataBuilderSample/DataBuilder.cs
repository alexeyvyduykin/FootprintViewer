using FootprintViewer.Data;
using FootprintViewer.Data.DataManager.Sources;

namespace JsonDataBuilderSample
{
    public class DataBuilder
    {
        public DataBuilder()
        {

        }

        public static async Task<IList<Footprint>> CreateRandomFootprints(int count)
        {
            var footprintSource = new FootprintRandomSource()
            {
                GenerateCount = count
            };

            return (IList<Footprint>)await footprintSource.GetValuesAsync();
        }

        public static async Task<IList<GroundTarget>> CreateRandomGroundTargets(int count)
        {
            var groundTargetsSource = new GroundTargetRandomSource()
            {
                GenerateCount = count
            };

            return (IList<GroundTarget>)await groundTargetsSource.GetValuesAsync();
        }

        public static async Task<IList<Satellite>> CreateRandomSatellites(int count)
        {
            var satellitesSource = new SatelliteRandomSource()
            {
                GenerateCount = count
            };

            return (IList<Satellite>)await satellitesSource.GetValuesAsync();
        }

        public static async Task<IList<Satellite>> CreateRandomGroundStations(int count)
        {
            var groundStationsSource = new GroundStationRandomSource()
            {
                GenerateCount = (count > 6) ? 6 : count
            };

            return (IList<Satellite>)await groundStationsSource.GetValuesAsync();
        }
    }
}
