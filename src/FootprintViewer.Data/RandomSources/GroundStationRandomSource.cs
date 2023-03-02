using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;

namespace FootprintViewer.Data.RandomSources;

public class GroundStationRandomSource : BaseRandomSource
{
    private List<GroundStation>? _groundStations;
    private static readonly List<GroundStation> _allGroundStations;

    static GroundStationRandomSource()
    {
        _allGroundStations = new List<GroundStation>()
        {
            new GroundStation() { Name = "Москва",      Center = new Point( 37.38, 55.56), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Новосибирск", Center = new Point( 82.57, 54.59), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Хабаровск",   Center = new Point(135.04, 48.29), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Шпицберген",  Center = new Point(    21, 78.38), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Анадырь",     Center = new Point(177.31, 64.44), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Тикси",       Center = new Point(128.52, 71.38), Angles = new [] { 0.0, 6, 10, 11 } },
        };
    }

    public override async Task<IList<object>> GetValuesAsync()
    {
        return await Task.Run(() =>
        {
            _groundStations ??= BuildGroundStations(GenerateCount);

            return _groundStations.Cast<object>().ToList();
        });
    }

    private static List<GroundStation> BuildGroundStations(int generateCount)
    {
        return _allGroundStations.Take(generateCount).ToList();
    }
}
