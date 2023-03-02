using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using System.Reflection;

namespace JsonDataBuilderSample;

internal class Program
{
    static async Task Main(string[] _)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        var footprintPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "Footprints.json"));
        var satellitePath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "Satellites.json"));
        var groundTargetPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "GroundTargets.json"));
        var groundStationPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "GroundStations.json"));
        var plannedSchedulePath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Output", "PlannedSchedule.json"));

        var satellites = await RandomModelBuilder.BuildRandomSatellitesAsync(5);
        var groundStations = await RandomModelBuilder.BuildRandomGroundStationsAsync(6);
        var footprints = await RandomModelBuilder.BuildRandomFootprintsAsync(satellites, 2000);
        var groundTargets = await RandomModelBuilder.BuildRandomGroundTargetsAsync(footprints, 5000);
        var plannedSchedule = await PlannedScheduleBuilder.BuildAsync(satellites, groundTargets, groundStations, footprints);

        //    JsonHelper.SerializeToFile(footprintPath, footprints);
        var footprintSuccess = JsonHelper.Verified<IList<Footprint>>(footprintPath);
        Console.WriteLine($"File {Path.GetFileName(footprintPath)} is build and verified = {footprintSuccess}.");

        //    JsonHelper.SerializeToFile(satellitePath, satellites);
        var satelliteSuccess = JsonHelper.Verified<IList<Satellite>>(satellitePath);
        Console.WriteLine($"File {Path.GetFileName(satellitePath)} is build and verified = {satelliteSuccess}.");

        //    JsonHelper.SerializeToFile(groundStationPath, groundStations);
        var groundStationSuccess = JsonHelper.Verified<IList<GroundStation>>(groundStationPath);
        Console.WriteLine($"File {Path.GetFileName(groundStationPath)} is build and verified = {groundStationSuccess}.");

        //    JsonHelper.SerializeToFile(groundTargetPath, groundTargets);
        var groundTargetSuccess = JsonHelper.Verified<IList<GroundTarget>>(groundTargetPath);
        Console.WriteLine($"File {Path.GetFileName(groundTargetPath)} is build and verified = {groundTargetSuccess}.");

        JsonHelper.SerializeToFile(plannedSchedulePath, plannedSchedule);
        var plannedScheduleSuccess = JsonHelper.Verified<PlannedScheduleResult>(plannedSchedulePath);
        Console.WriteLine($"File {Path.GetFileName(plannedSchedulePath)} is build and verified = {plannedScheduleSuccess}.");
    }
}