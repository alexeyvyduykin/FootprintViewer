using System.Reflection;

namespace JsonDataBuilderSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            var footprintPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "Footprints.json"));
            var satellitePath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "Satellites.json"));
            var groundTargetPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "GroundTargets.json"));
            var groundStationPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "GroundStations.json"));

            var satellites = await DataBuilder.CreateRandomSatellites(5);
            var groundStations = await DataBuilder.CreateRandomGroundStations(6);
            var footprints = await DataBuilder.CreateRandomFootprints(satellites, 2000);
            var groundTargets = await DataBuilder.CreateRandomGroundTargets(footprints, 5000);

            JsonHelper.SerializeToFile(footprintPath, footprints);
            JsonHelper.SerializeToFile(satellitePath, satellites);
            JsonHelper.SerializeToFile(groundStationPath, groundStations);
            JsonHelper.SerializeToFile(groundTargetPath, groundTargets);
        }
    }
}